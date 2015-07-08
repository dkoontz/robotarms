using UnityEngine;
using System.Collections.Generic;
using RobotArms;

namespace RobotArms.BuiltIn {
	public class Trigger2DTracker : RobotArmsComponent {

		public List<Collider2D> Collisions = new List<Collider2D>();

		public void OnTriggerEnter2D(Collider2D other) {
			Collisions.Add(other);
		}

		public void OnTriggerExit2D(Collider2D other) {
			Collisions.Remove(other);
		}

		public new void OnDisable() {
			base.OnDisable();
			Collisions.Clear();
		}
	}
}