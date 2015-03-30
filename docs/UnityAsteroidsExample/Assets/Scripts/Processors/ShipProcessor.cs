using System.Linq;
using UnityEngine;
using RobotArms;

[ProcessorOptions(typeof(Ship), typeof(PlayerInput), typeof(VectoredMovement), typeof(Trigger2D))]
public class ShipMovementProcessor : RobotArmsProcessor {
	
	public override void Process (GameObject entity) {
		var input = entity.GetComponent<PlayerInput>();
		var ship = entity.GetComponent<Ship>();
		var movement = entity.GetComponent<VectoredMovement>();
		var trigger = entity.GetComponent<Trigger2D>();
		var destroyable = entity.GetComponent<Destroyable>();

		if (ship.Fuel > 0) {
			if (input.Thrust) {
				movement.Velocity += entity.transform.up * ship.ThrustForce * Time.deltaTime;
				if (movement.Velocity.magnitude > ship.MaxSpeed) {
					movement.Velocity = movement.Velocity.normalized * ship.MaxSpeed;
				}

				ship.Fuel -= ship.FuelConsumptionPerSecond * Time.deltaTime;
			}

			entity.transform.Rotate(0, 0, -input.Rotation * ship.RotationSpeed * Time.deltaTime);
		}

		if (input.Fire) {
			RobotArmsUtils.RunAtEndOfFrame(() => {
				var projectile = GameObject.Instantiate(ship.Projectile, ship.transform.position, ship.transform.rotation) as GameObject;
				var projectileMovement = projectile.GetComponent<VectoredMovement>();
				projectileMovement.Velocity = projectile.transform.up * projectile.GetComponent<Projectile>().Speed;
			});
		}

		if (trigger.Colliders.Any(c => destroyable.DestroyWhenTouchingTag.Contains(c.tag))) {

			RobotArmsUtils.RunAtEndOfFrame(() => {
				GameObject.Instantiate(destroyable.DestroyedEffect, entity.transform.position, entity.transform.rotation);	
				GameObject.Destroy(entity);
			});
		}
	}
}