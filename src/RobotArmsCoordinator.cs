using System;
using System.Collections;
using System.Collections.Generic;
using GoodStuff.NaturalLanguage;
using System.Linq;
using UnityEngine;

namespace RobotArms {
	public class RobotArmsCoordinator : MonoBehaviour, IRobotArmsCoordinator {
		public MonoBehaviour Blackboard;
		[HideInInspector] public List<string> EnabledProcessorTags = new List<string> { "Untagged" };

		RobotArmsProcessor[] processors;
		RobotArmsProcessor[] updateProcessors;
		RobotArmsProcessor[] fixedUpdateProcessors;
		RobotArmsProcessor[] lateUpdateProcessors;
		Dictionary<RobotArmsProcessor, HashSet<GameObject>> entitiesForProcessors;
		HashSet<RobotArmsComponent> components = new HashSet<RobotArmsComponent>();
		Queue<GameObject> entitiesWithComponentsThatWereRemoved = new Queue<GameObject>();
		Queue<Action> actionsToRunAtEndOfCurrentUpdateType = new Queue<Action>();
		Queue<Action> actionsToRunAtEndOfFrame = new Queue<Action>();

		public void Awake() {
			if (Blackboard == null) {
				throw new MissingComponentException("A blackboard component was not specified");
			}

			var processorTypes = AppDomain.CurrentDomain.GetAssemblies().SelectMany(
				assembly => assembly.GetTypes().Where(
					type => type.IsSubclassOf(typeof(RobotArmsProcessor))));

			processors = processorTypes
				.Where(type => EnabledProcessorTags.Contains((type.GetCustomAttributes(typeof(ProcessorOptionsAttribute), true)[0] as ProcessorOptionsAttribute).Tag))
				.Select(type => Activator.CreateInstance(type) as RobotArmsProcessor)
				.OrderBy(p => -p.Options.Priority)
				.ToArray();

			processors.Each(p => {
				p.Coordinator = this;
				p.Blackboard = Blackboard;
			});
				
			entitiesForProcessors = new Dictionary<RobotArmsProcessor, HashSet<GameObject>>(processors.Length);
			processors.Each(p => entitiesForProcessors[p] = new HashSet<GameObject>());
			updateProcessors = processors.Where(p => p.Options.Phase == UpdateType.Update).ToArray();
			fixedUpdateProcessors = processors.Where(p => p.Options.Phase == UpdateType.FixedUpdate).ToArray();
			lateUpdateProcessors = processors.Where(p => p.Options.Phase == UpdateType.LateUpdate).ToArray();

			RobotArmsComponent.RobotArmsCoordinator = this;

			StartCoroutine(EndOfFrameActionProcessor());
		}

		// TODO: Add ability to add and remove tags that are active
		// this should cause the set of processors to change

		public void RegisterComponent(RobotArmsComponent component) {
			components.Add(component);
			processors.Where(p => p.IsInterestedIn(component.gameObject)).Each(p => entitiesForProcessors[p].Add(component.gameObject));
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
			RemovePendingComponents();

			updateProcessors.Each(p => {
				if (p.IsActive == null || p.IsActive()) {
					p.ProcessAll(entitiesForProcessors[p]);
				}
			});

			RunEndOfCurrentUpdateActions();
		}

		public void FixedUpdate() {
			RemovePendingComponents();

			fixedUpdateProcessors.Each(p => {
				if (p.IsActive == null || p.IsActive()) {
					p.ProcessAll(entitiesForProcessors[p]);
				}
			});

			RunEndOfCurrentUpdateActions();
		}

		public void LateUpdate() {
			RemovePendingComponents();

			lateUpdateProcessors.Each(p => {
				if (p.IsActive == null || p.IsActive()) {
					p.ProcessAll(entitiesForProcessors[p]);
				}
			});

			RunEndOfCurrentUpdateActions();
		}

		void RemovePendingComponents() {
			while (entitiesWithComponentsThatWereRemoved.Count > 0) { 
				var entity = entitiesWithComponentsThatWereRemoved.Dequeue();
				entitiesForProcessors.Each((processor, entitiesForProcessor) => {
					if (!processor.IsInterestedIn(entity)) {
						entitiesForProcessor.Remove(entity);
					}
				});
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
	}
}