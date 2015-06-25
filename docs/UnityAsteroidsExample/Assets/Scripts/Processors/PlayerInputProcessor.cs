using UnityEngine;
using RobotArms;

public class PlayerInputProcessor : RobotArmsProcessor<PlayerInput> {
	
	public override void Process (GameObject entity, PlayerInput input) {
		input.Thrust = Input.GetAxis("Vertical") > 0;
		input.Rotation = Input.GetAxis("Horizontal");
		input.Fire = Input.GetKeyDown(KeyCode.Space);
	}
}