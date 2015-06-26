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

	// Priority is ordered from smallest to largest to match Unity's script execution ordering
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
	public class ProcessorOptionsAttribute : Attribute {
		public readonly UpdateType Phase;
		public readonly int Priority;
		public readonly string Tag;

		public ProcessorOptionsAttribute(string tag, UpdateType phase, int priority) {
			Phase = phase;
			Priority = priority;
			Tag = tag;
		}

		public ProcessorOptionsAttribute() : this(RobotArmsCoordinator.DEFAULT_TAG, UpdateType.Update, 0) {}
		public ProcessorOptionsAttribute(string tag) : this(tag, UpdateType.Update, 0) {}
		public ProcessorOptionsAttribute(UpdateType phase) : this(RobotArmsCoordinator.DEFAULT_TAG, phase, 0) {}
		public ProcessorOptionsAttribute(string tag, UpdateType phase) : this(tag, phase, 0) {}
		public ProcessorOptionsAttribute(int priority) : this(RobotArmsCoordinator.DEFAULT_TAG, UpdateType.Update, priority) {}
		public ProcessorOptionsAttribute(string tag, int priority) : this(tag, UpdateType.Update, priority) {}
	}

	public abstract class RobotArmsProcessor<T1> : RobotArmsProcessor where T1 : Component {
		public virtual void Initialize(GameObject entity, T1 component1) {}
		public virtual void Process(GameObject entity, T1 component1) {}

		public override void InitializeAll(List<EntityAndComponents> entities) {
			foreach (var entityAndComponents in entities) {
				Initialize(
					entityAndComponents.Entity, 
					(T1)entityAndComponents.Components[0]);
			}
		}

		public override void ProcessAll(List<EntityAndComponents> entities) {
			foreach (var entityAndComponents in entities) {
				Process(
					entityAndComponents.Entity, 
					(T1)entityAndComponents.Components[0]);
			}
		}
	}

	public abstract class RobotArmsProcessor<T1, T2> : RobotArmsProcessor where T1 : Component where T2 : Component {
		public virtual void Initialize(GameObject entity, T1 component1, T2 component2) {}
		public virtual void Process(GameObject entity, T1 component1, T2 component2) {}

		public override void InitializeAll(List<EntityAndComponents> entities) {
			foreach (var entityAndComponents in entities) {
				Initialize(
					entityAndComponents.Entity, 
					(T1)entityAndComponents.Components[0],
					(T2)entityAndComponents.Components[1]);
			}
		}

		public override void ProcessAll(List<EntityAndComponents> entities) {
			foreach (var entityAndComponents in entities) {
				Process(
					entityAndComponents.Entity, 
					(T1)entityAndComponents.Components[0],
					(T2)entityAndComponents.Components[1]);
			}
		}
	}

	public abstract class RobotArmsProcessor<T1, T2, T3> : RobotArmsProcessor where T1 : Component where T2 : Component where T3 : Component {
		public virtual void Initialize(GameObject entity, T1 component1, T2 component2, T3 component) {}
		public virtual void Process(GameObject entity, T1 component1, T2 component2, T3 component) {}

		public override void InitializeAll(List<EntityAndComponents> entities) {
			foreach (var entityAndComponents in entities) {
				Initialize(
					entityAndComponents.Entity, 
					(T1)entityAndComponents.Components[0],
					(T2)entityAndComponents.Components[1],
					(T3)entityAndComponents.Components[2]);
			}
		}

		public override void ProcessAll(List<EntityAndComponents> entities) {
			foreach (var entityAndComponents in entities) {
				Process(
					entityAndComponents.Entity, 
					(T1)entityAndComponents.Components[0],
					(T2)entityAndComponents.Components[1],
					(T3)entityAndComponents.Components[2]);
			}
		}
	}

	public abstract class RobotArmsProcessor<T1, T2, T3, T4> : RobotArmsProcessor where T1 : Component where T2 : Component where T3 : Component where T4 : Component {
		public virtual void Initialize(GameObject entity, T1 component1, T2 component2, T3 component, T4 component4) {}
		public virtual void Process(GameObject entity, T1 component1, T2 component2, T3 component, T4 component4) {}

		public override void InitializeAll(List<EntityAndComponents> entities) {
			foreach (var entityAndComponents in entities) {
				Initialize(
					entityAndComponents.Entity, 
					(T1)entityAndComponents.Components[0],
					(T2)entityAndComponents.Components[1],
					(T3)entityAndComponents.Components[2],
					(T4)entityAndComponents.Components[3]);
			}
		}

		public override void ProcessAll(List<EntityAndComponents> entities) {
			foreach (var entityAndComponents in entities) {
				Process(
					entityAndComponents.Entity, 
					(T1)entityAndComponents.Components[0],
					(T2)entityAndComponents.Components[1],
					(T3)entityAndComponents.Components[2],
					(T4)entityAndComponents.Components[3]);
			}
		}
	}

	public abstract class RobotArmsProcessor<T1, T2, T3, T4, T5> : RobotArmsProcessor where T1 : Component where T2 : Component where T3 : Component where T4 : Component where T5 : Component {
		public virtual void Initialize(GameObject entity, T1 component1, T2 component2, T3 component, T4 component4, T5 component5) {}
		public virtual void Process(GameObject entity, T1 component1, T2 component2, T3 component, T4 component4, T5 component5) {}

		public override void InitializeAll(List<EntityAndComponents> entities) {
			foreach (var entityAndComponents in entities) {
				Initialize(
					entityAndComponents.Entity, 
					(T1)entityAndComponents.Components[0],
					(T2)entityAndComponents.Components[1],
					(T3)entityAndComponents.Components[2],
					(T4)entityAndComponents.Components[3],
					(T5)entityAndComponents.Components[4]);
			}
		}

		public override void ProcessAll(List<EntityAndComponents> entities) {
			foreach (var entityAndComponents in entities) {
				Process(
					entityAndComponents.Entity, 
					(T1)entityAndComponents.Components[0],
					(T2)entityAndComponents.Components[1],
					(T3)entityAndComponents.Components[2],
					(T4)entityAndComponents.Components[3],
					(T5)entityAndComponents.Components[4]);
			}
		}
	}

	public abstract class RobotArmsProcessor {
		public ProcessorOptionsAttribute Options { get; private set; }
		public Func<bool> IsActive { get; protected set; }
		public Type[] RequiredTypes;
		internal object Blackboard { private get; set; }

		protected RobotArmsProcessor() {
			var attributes = GetType().GetCustomAttributes(typeof(ProcessorOptionsAttribute), true);

			Options = attributes.Length == 0 
				? new ProcessorOptionsAttribute() 
				: GetType().GetCustomAttributes(typeof(ProcessorOptionsAttribute), true)[0] as ProcessorOptionsAttribute;

			RequiredTypes = GetType().BaseType.GetGenericArguments();
		}

		public bool IsInterestedIn(GameObject entity) {

			if (entity == null) {
				return false;
			}

			return RequiredTypes.All(type => {
				var component = entity.GetComponent(type) as MonoBehaviour;
				return component != null && component.enabled;
			});
		}

		public abstract void InitializeAll(List<EntityAndComponents> entities);

		public abstract void ProcessAll(List<EntityAndComponents> entities);

		public T GetBlackboard<T>() where T : MonoBehaviour {
			return (T)Blackboard;
		}
	}
}