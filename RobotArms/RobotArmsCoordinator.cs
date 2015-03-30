// This project is licensed under The MIT License (MIT)
// 
// Copyright 2014 David Koontz, Trenton Kennedy
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// 	 The above copyright notice and this permission notice shall be included in
// 	 all copies or substantial portions of the Software.
//   
// 	 THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// 	 IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// 	 FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// 	 AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// 	 LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// 	 OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// 	 THE SOFTWARE.
//   
// 	 Please direct questions, patches, and suggestions to the project page at
// 	 https://bitbucket.org/dkoontz/robotarms

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RobotArms {
	public class RobotArmsCoordinator : MonoBehaviour, IRobotArmsCoordinator {
		public const string DEFAULT_TAG = "Untagged";

		public MonoBehaviour Blackboard;
		[HideInInspector] public List<string> EnabledProcessorTags = new List<string> { DEFAULT_TAG };

		RobotArmsProcessor[] processors;
		RobotArmsProcessor[] updateProcessors;
		RobotArmsProcessor[] fixedUpdateProcessors;
		RobotArmsProcessor[] lateUpdateProcessors;

		// We can't use a HashSet for the entity list due to this bug
		// http://fogbugz.unity3d.com/default.asp?676743_ro5i82m19hb8tom8
		Dictionary<RobotArmsProcessor, List<GameObject>> entitiesForProcessors;
		Dictionary<RobotArmsProcessor, List<GameObject>> entitiesForProcessorsToInitialize;
	    
		readonly HashSet<RobotArmsComponent> components = new HashSet<RobotArmsComponent>();
	    readonly Queue<GameObject> entitiesWithComponentsThatWereRemoved = new Queue<GameObject>();
	    readonly Queue<Action> actionsToRunAtEndOfCurrentUpdateType = new Queue<Action>();
	    readonly Queue<Action> actionsToRunAtEndOfFrame = new Queue<Action>();

		public void Awake() {
			if (Blackboard == null) {
				Debug.LogWarning("A blackboard component was not specified");
			}

			if (EnabledProcessorTags == null) {
				EnabledProcessorTags = new List<string> { "Untagged" };
			}

			var processorTypes = AppDomain.CurrentDomain.GetAssemblies().SelectMany(
				assembly => assembly.GetTypes().Where(
					type => type.IsSubclassOf(typeof(RobotArmsProcessor))));

			processors = processorTypes
				.Where(type => EnabledProcessorTags.Contains((type.GetCustomAttributes(typeof(ProcessorOptionsAttribute), true)[0] as ProcessorOptionsAttribute).Tag))
				.Select(type => Activator.CreateInstance(type) as RobotArmsProcessor)
				.OrderBy(p => p.Options.Priority)
				.ToArray();

			foreach (var p in processors) {
				p.Blackboard = Blackboard;
			}

			entitiesForProcessors = new Dictionary<RobotArmsProcessor, List<GameObject>>(processors.Length);
			entitiesForProcessorsToInitialize = new Dictionary<RobotArmsProcessor, List<GameObject>>(processors.Length);

			foreach (var p in processors) {
				entitiesForProcessors[p] = new List<GameObject>();
				entitiesForProcessorsToInitialize[p] = new List<GameObject>();
			}
			updateProcessors = processors.Where(p => p.Options.Phase == UpdateType.Update).ToArray();
			fixedUpdateProcessors = processors.Where(p => p.Options.Phase == UpdateType.FixedUpdate).ToArray();
			lateUpdateProcessors = processors.Where(p => p.Options.Phase == UpdateType.LateUpdate).ToArray();

			RobotArmsUtils.Coordinator = this;
			RobotArmsComponent.RobotArmsCoordinator = this;

			StartCoroutine(EndOfFrameActionProcessor());
		}

		// TODO: Add ability to add and remove tags that are active
		// this should cause the set of processors to change

		public void RegisterComponent(RobotArmsComponent component) {
			components.Add(component);
			// Checking the specific required types prevents calling 
			// Initialize/InitializeAll on processors when a component is added
			// that the processor does not care about while the entity
			// IS of interest to the processor
//			Debug.Log("Registering component: " + component.GetType() + " from game object " + component.gameObject.name);
			foreach (var p in processors.Where(p => p.Options.RequiredTypes.Contains(component.GetType()) && p.IsInterestedIn(component.gameObject))) {
				AddDistinct(entitiesForProcessors[p], component.gameObject);
				AddDistinct(entitiesForProcessorsToInitialize[p], component.gameObject);
			}

		}

		public void UnregisterComponent(RobotArmsComponent component) {
			components.Remove(component);
			entitiesWithComponentsThatWereRemoved.Enqueue(component.gameObject);
		}

		public void RunAtEndOfFrame(Action action) {
			actionsToRunAtEndOfFrame.Enqueue(action);
		}

		public void RunAtEndOfCurrentUpdateType(Action action) {
			actionsToRunAtEndOfCurrentUpdateType.Enqueue(action);
		}

		public T[] GetAllComponentsOfType<T>() where T : RobotArmsComponent {
			return components.OfType<T>().ToArray();
		}

		public void Update() {
			RunProcessors(updateProcessors);
		}

		public void FixedUpdate() {
			RunProcessors(fixedUpdateProcessors);
		}

		public void LateUpdate() {
			RunProcessors(lateUpdateProcessors);
		}

		void RunProcessors(RobotArmsProcessor[] robotArmsProcessors) {
			RemovePendingComponents();

			foreach (var p in robotArmsProcessors) {
				if ((p.IsActive == null || p.IsActive()) && entitiesForProcessorsToInitialize[p].Count > 0) {
					p.InitializeAll(entitiesForProcessorsToInitialize[p]);
					entitiesForProcessorsToInitialize[p].Clear();
				}
			}

			foreach (var p in robotArmsProcessors) {
				if (p.IsActive == null || p.IsActive()) {
					var entities = entitiesForProcessors[p];
					entities.RemoveAll(e => e == null);
					p.ProcessAll(entities);
				}
			}

			RunEndOfCurrentUpdateActions();
		}

		void RemovePendingComponents() {
			while (entitiesWithComponentsThatWereRemoved.Count > 0) { 
				var entity = entitiesWithComponentsThatWereRemoved.Dequeue();
				foreach (var kvp in entitiesForProcessors) {
					var processor = kvp.Key;
					var entitiesForProcessor = kvp.Value;

					entitiesForProcessor.RemoveAll(e => e == null);
					if (entitiesForProcessor.Contains(entity) && !processor.IsInterestedIn(entity)) {
						entitiesForProcessor.Remove(entity);
					}
				}
			}
		}

		void RunEndOfCurrentUpdateActions() {
			while (actionsToRunAtEndOfCurrentUpdateType.Count > 0) {
				actionsToRunAtEndOfCurrentUpdateType.Dequeue()();
			}
		}

		IEnumerator EndOfFrameActionProcessor() {
			while (true) {
				yield return new WaitForEndOfFrame();
				while (actionsToRunAtEndOfFrame.Count > 0) {
					actionsToRunAtEndOfFrame.Dequeue()();
				}
			}
		}

		static void AddDistinct<T>(IList<T> list, T element) {
			if (!list.Contains(element)) {
				list.Add(element);
			}
		}
	}
}