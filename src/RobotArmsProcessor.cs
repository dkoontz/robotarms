using System;
using System.Collections.Generic;
using System.Linq;
using GoodStuff.NaturalLanguage;
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

			requiredTypes.Each(type => {
				if (!typeof(Component).IsAssignableFrom(type)) {
					throw new ArgumentException("Type {0} is not a UnityEngine.Component".Fmt(type));
				}
			});
		}

		public ProcessorOptionsAttribute(params Type[] requiredTypes) : this ("Untagged", UpdateType.Update, 0, requiredTypes) { }
		public ProcessorOptionsAttribute(string tag, params Type[] requiredTypes) : this (tag, UpdateType.Update, 0, requiredTypes) { }
		public ProcessorOptionsAttribute(UpdateType phase, params Type[] requiredTypes) : this("Untagged", phase, 0, requiredTypes) { }
		public ProcessorOptionsAttribute(string tag, UpdateType phase, params Type[] requiredTypes) : this(tag, phase, 0, requiredTypes) { }
		public ProcessorOptionsAttribute(int priority = 0, params Type[] requiredTypes) : this ("Untagged", UpdateType.Update, priority, requiredTypes) { }
		public ProcessorOptionsAttribute(string tag, int priority = 0, params Type[] requiredTypes) : this (tag, UpdateType.Update, priority, requiredTypes) { }
	}

	public class RobotArmsProcessor {
		public ProcessorOptionsAttribute Options { get; private set; }
		public Func<bool> IsActive { get; protected set; }
		internal IRobotArmsCoordinator Coordinator { private get; set; }
		internal object Blackboard { private get; set; }

		public RobotArmsProcessor() {
			if (GetType().GetCustomAttributes(typeof(ProcessorOptionsAttribute), true).IsEmpty()) {
				throw new Exception(string.Format("RobotArmsProcessor {0} requires a ProcessorOptions attribute", GetType()));
			}

			Options = GetType().GetCustomAttributes(typeof(ProcessorOptionsAttribute), true)[0] as ProcessorOptionsAttribute;
		}

		public bool IsInterestedIn(GameObject entity) {
			return Options.RequiredTypes.All(type => entity != null && entity.GetComponent(type) != null);
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

		public void RunAtEndOfFrame(Action action) {
			Coordinator.RunAtEndOfFrame(action);
		}

		public void RunAtEndOfCurrentUpdateType(Action action) {
			Coordinator.RunAtEndOfCurrentUpdateType(action);
		}

		public void DestroyComponent(RobotArmsComponent component) {
			Coordinator.RunAtEndOfCurrentUpdateType(() => GameObject.Destroy(component));
		}

		public void DestroyGameObject(GameObject gameObject) {
			Coordinator.RunAtEndOfCurrentUpdateType(() => GameObject.Destroy(gameObject));
		}

		public T[] GetAllComponentsOfType<T>() where T : RobotArmsComponent {
			return Coordinator.GetAllComponentsOfType<T>();
		}
	}
}