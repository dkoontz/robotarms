using UnityEngine;
using System.Collections.Generic;
using RobotArms;

namespace RobotArms.BuiltIn {
	public class CollisionTracker : RobotArmsComponent {

		public List<Collision> Collisions = new List<Collision>();

		public void OnCollisionEnter(Collision other) {
			Collisions.Add(other);
		}

		public void OnCollisionExit(Collision other) {
			Collisions.Remove(other);
		}

		public new void OnDisable() {
			base.OnDisable();
			Collisions.Clear();
		}
	}
}