using System;
using UnityEngine;
using RobotArms;

namespace RobotArms {
	public class RobotArmsComponent : MonoBehaviour {
		public static IRobotArmsCoordinator RobotArmsCoordinator;

		// Set this to false during Initialize or Awake to prevent the component
		// from being registered with RobotArms. This is useful for situations
		// where the object is created but not yet active such as with an object pool
		protected bool autoRegister = true;

		public void Start() {
			Initialize();

			if (autoRegister) {
				RobotArmsCoordinator.RegisterComponent(this);
			}
		}

		public void OnDestroy() {
			RobotArmsCoordinator.UnregisterComponent(this);
		}

		/// <summary>
		/// Called before component is registered with any processors.
		/// </summary>
		protected virtual void Initialize() { }
	}
}