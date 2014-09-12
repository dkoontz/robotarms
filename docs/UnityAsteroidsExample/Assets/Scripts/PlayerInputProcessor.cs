using UnityEngine;
using RobotArms;

[ProcessorOptions(typeof(PlayerInput))]
public class PlayerInputProcessor : RobotArmsProcessor {
	public override void Process (GameObject entity) {
		var input = entity.GetComponent<PlayerInput>();
		input.Thrust = Input.GetAxis("Vertical") > 0;
		input.Rotation = Input.GetAxis("Horizontal");
	}
}