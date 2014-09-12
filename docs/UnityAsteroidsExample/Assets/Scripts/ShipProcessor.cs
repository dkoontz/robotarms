using UnityEngine;
using RobotArms;

[ProcessorOptions(typeof(Ship), typeof(PlayerInput), typeof(VectoredMovement))]
public class ShipMovementProcessor : RobotArmsProcessor {
	public override void Process (GameObject entity) {
		var input = entity.GetComponent<PlayerInput>();
		var ship = entity.GetComponent<Ship>();
		var movement = entity.GetComponent<VectoredMovement>();

		if (ship.Fuel > 0) {
			if (input.Thrust) {
				movement.Velocity += entity.transform.up * ship.ThrustForce * Time.deltaTime;
				ship.Fuel -= ship.FuelConsumptionPerSecond * Time.deltaTime;
			}

			entity.transform.Rotate(0, 0, -input.Rotation * ship.RotationSpeed * Time.deltaTime);
		}
	}
}