using UnityEngine;
using System.Collections;
using RobotArms;

namespace RobotArms.BuiltIn {
	[ProcessorOptions(typeof(ContinuousMovement))]
	public class ContinuousMovementProcessor : RobotArmsProcessor {

		public override void Process(GameObject entity) {
			var movement = entity.GetComponent<ContinuousMovement>();
			var transform = movement.transform;

			transform.Translate(movement.Direction * movement.Speed * Time.deltaTime);
		}
	}
}