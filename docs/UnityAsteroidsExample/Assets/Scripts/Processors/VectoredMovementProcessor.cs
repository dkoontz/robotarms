using UnityEngine;
using RobotArms;

public class VectoredMovementProcessor : RobotArmsProcessor<VectoredMovement> {
	
	public override void Process (GameObject entity, VectoredMovement movement) {
		entity.transform.Translate(movement.Velocity * Time.deltaTime, Space.World);
		entity.transform.Rotate(Vector3.forward * movement.Rotation);
	}
}