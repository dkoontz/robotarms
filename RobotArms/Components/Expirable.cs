using UnityEngine;
using System.Collections;
using RobotArms;

namespace RobotArms.BuiltIn {
	public class Expirable : RobotArmsComponent {

		public float TimeRemaining;
		public GameObject Target;

		protected override void Initialize() {
			if (Target == null) {
				Target = gameObject;
			}
		}
	}
}