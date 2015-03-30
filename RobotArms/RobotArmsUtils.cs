using System;
using UnityEngine;

namespace RobotArms {
	public static class RobotArmsUtils {
		public static RobotArmsCoordinator Coordinator;

		public static void RunAtEndOfFrame(Action action) {
			Coordinator.RunAtEndOfFrame(action);
		}

		public static void RunAtEndOfCurrentUpdateType(Action action) {
			Coordinator.RunAtEndOfCurrentUpdateType(action);
		}

		public static void DestroyComponent(RobotArmsComponent component) {
			Coordinator.RunAtEndOfFrame(() => GameObject.Destroy(component));
		}

		public static void DestroyGameObject(GameObject gameObject) {
			Coordinator.RunAtEndOfFrame(() => GameObject.Destroy(gameObject));
		}

		public static T[] GetAllComponentsOfType<T>() where T : RobotArmsComponent {
			return Coordinator.GetAllComponentsOfType<T>();
		}
	}
}