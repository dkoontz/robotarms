using System;
using UnityEngine;

namespace RobotArms {
	public interface IRobotArmsCoordinator {
		void RegisterComponent(RobotArmsComponent component);
		void UnregisterComponent(RobotArmsComponent component);
		void RunAtEndOfFrame(Action action);
		void RunAtEndOfCurrentUpdateType(Action action);
		void DestroyComponent(RobotArmsComponent component);
		void DestroyGameObject(GameObject gameObject);
	}
}