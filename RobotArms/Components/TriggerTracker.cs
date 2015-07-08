using UnityEngine;
using System.Collections.Generic;
using RobotArms;

namespace RobotArms.BuiltIn {
	public class TriggerTracker : RobotArmsComponent {

		public List<Collider> Colliders = new List<Collider>();

		public void OnTriggerEnter(Collider other) {
			Colliders.Add(other);
		}

		public void OnTriggerExit(Collider other) {
			Colliders.Remove(other);
		}

		public new void OnDisable() {
			base.OnDisable();
			Colliders.Clear();
		}
	}
}