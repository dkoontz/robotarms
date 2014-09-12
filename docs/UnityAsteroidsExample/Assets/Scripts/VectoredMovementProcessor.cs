using UnityEngine;
using RobotArms;

[ProcessorOptions(typeof(VectoredMovement))]
public class VectoredMovementProcessor : RobotArmsProcessor {
	public override void Process (GameObject entity) {
		var movement = entity.GetComponent<VectoredMovement>();
		entity.transform.Translate(movement.Velocity * Time.deltaTime, Space.World);
	}
}