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
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RobotArms {
	public enum UpdateType {
		Update, FixedUpdate, LateUpdate
	}

	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
	public class ProcessorOptionsAttribute : Attribute { 
		public readonly UpdateType Phase;
		public readonly int Priority;
		public readonly Type[] RequiredTypes;
		public readonly string Tag;

		public ProcessorOptionsAttribute(string tag, UpdateType phase, int priority, params Type[] requiredTypes) {
			Phase = phase;
			Priority = priority;
			RequiredTypes = requiredTypes;
			Tag = tag;

			foreach (var type in requiredTypes) {
				if (!typeof(Component).IsAssignableFrom(type)) {
					throw new ArgumentException(string.Format("Type {0} is not a UnityEngine.Component", type));
				}
			}
		}

		public ProcessorOptionsAttribute(params Type[] requiredTypes) : this(RobotArmsCoordinator.DEFAULT_TAG, UpdateType.Update, 0, requiredTypes) {}
		public ProcessorOptionsAttribute(string tag, params Type[] requiredTypes) : this(tag, UpdateType.Update, 0, requiredTypes) {}
		public ProcessorOptionsAttribute(UpdateType phase, params Type[] requiredTypes) : this(RobotArmsCoordinator.DEFAULT_TAG, phase, 0, requiredTypes) {}
		public ProcessorOptionsAttribute(string tag, UpdateType phase, params Type[] requiredTypes) : this(tag, phase, 0, requiredTypes) {}
		public ProcessorOptionsAttribute(int priority, params Type[] requiredTypes) : this(RobotArmsCoordinator.DEFAULT_TAG, UpdateType.Update, priority, requiredTypes) {}
		public ProcessorOptionsAttribute(string tag, int priority, params Type[] requiredTypes) : this(tag, UpdateType.Update, priority, requiredTypes) {}
	}

	public class RobotArmsProcessor {
		public ProcessorOptionsAttribute Options { get; private set; }
		public Func<bool> IsActive { get; protected set; }
		internal object Blackboard { private get; set; }

		public RobotArmsProcessor() {
			if (GetType().GetCustomAttributes(typeof(ProcessorOptionsAttribute), true).Length == 0) {
				throw new Exception(string.Format("RobotArmsProcessor {0} requires a ProcessorOptions attribute", GetType()));
			}

			Options = GetType().GetCustomAttributes(typeof(ProcessorOptionsAttribute), true)[0] as ProcessorOptionsAttribute;
		}

		public bool IsInterestedIn(GameObject entity) {

			if (entity == null) {
				return false;
			}

			return Options.RequiredTypes.All(type => {
				var component = entity.GetComponent(type) as MonoBehaviour;
				return component != null && component.enabled;
			});
		}

		public virtual void Initialize(GameObject entity) { }

		public virtual void InitializeAll(IEnumerable<GameObject> entities) {
			foreach (var entity in entities) {
				Initialize(entity);
			}
		}

		public virtual void Process(GameObject entity) { }

		public virtual void ProcessAll(IEnumerable<GameObject> entities) {
			foreach (var entity in entities) {
				Process(entity);
			}
		}

		public T GetBlackboard<T>() where T : MonoBehaviour {
			return (T)Blackboard;
		}
	}
}