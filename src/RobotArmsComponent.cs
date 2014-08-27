using System;
using UnityEngine;
//using Artemis.Interface;
using RobotArms;

namespace RobotArms {
	public class RobotArmsComponent : MonoBehaviour { //, IComponent {
//		public static EntitySystemManager EntitySystemManager;
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
//			EntitySystemManager.AddComponent(gameObject, this);
//			EntitySystemManager.OnBeforeEntityProcessed += OnBeforeEntityProcessed;
//			EntitySystemManager.OnAfterEntityProcessed += OnAfterEntityProcessed;
		}

		public void OnDestroy() {
			Debug.Log("Destroying Component");
			RobotArmsCoordinator.UnregisterComponent(this);
		}

		/// <summary>
		/// Called before component is registered with any processors.
		/// </summary>
		protected virtual void Initialize() { }
//		protected virtual void OnBeforeEntityProcessed() { }
//		protected virtual void OnAfterEntityProcessed() { }
	}
}