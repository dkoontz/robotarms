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

		public ProcessorOptionsAttribute(UpdateType phase, int priority, params Type[] requiredTypes) {
			Phase = phase;
			Priority = priority;
			RequiredTypes = requiredTypes;

			requiredTypes.Each(type => {
				if (!typeof(Component).IsAssignableFrom(type)) {
					throw new ArgumentException("Type {0} is not a UnityEngine.Component".Fmt(type));
				}
			});
		}

		public ProcessorOptionsAttribute(params Type[] requiredTypes) : this (UpdateType.Update, 0, requiredTypes) { }
		public ProcessorOptionsAttribute(UpdateType phase, params Type[] requiredTypes) : this(phase, 0, requiredTypes) { }
		public ProcessorOptionsAttribute(int priority, params Type[] requiredTypes) : this (UpdateType.Update, priority, requiredTypes) { }
	}

	public class RobotArmsProcessor {
		public ProcessorOptionsAttribute Options { get; private set; }
		public Func<bool> IsActive { get; private set; }
		internal IRobotArmsCoordinator Coordinator { private get; set; }
		internal object Blackboard { private get; set; }

		public RobotArmsProcessor() {
			if (GetType().GetCustomAttributes(typeof(ProcessorOptionsAttribute), true).IsEmpty()) {
				throw new Exception("RobotArmsProcessor requires a ProcessorOptions attribute");
			}

			Options = GetType().GetCustomAttributes(typeof(ProcessorOptionsAttribute), true)[0] as ProcessorOptionsAttribute;}

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
	}
}