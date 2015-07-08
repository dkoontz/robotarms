using UnityEngine;
using System.Collections.Generic;
using RobotArms;

namespace RobotArms.BuiltIn {
	public class Collision2DTracker : RobotArmsComponent {

		public List<Collision2D> Collisions = new List<Collision2D>();

		public void OnCollisionEnter2D(Collision2D other) {
			Collisions.Add(other);
		}

		public void OnCollisionExit2D(Collision2D other) {
			Collisions.Remove(other);
		}

		public new void OnDisable() {
			base.OnDisable();
			Collisions.Clear();
		}
	}
}