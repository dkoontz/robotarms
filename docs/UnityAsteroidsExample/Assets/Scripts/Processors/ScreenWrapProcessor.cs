using UnityEngine;
using RobotArms;

public class ScreenWrapProcessor : RobotArmsProcessor<VectoredMovement> {

	public override void Process (GameObject entity, VectoredMovement vectoredMovement) {
		var viewportPosition = Camera.main.WorldToViewportPoint(vectoredMovement.transform.position);
		if (viewportPosition.x < 0) {
			viewportPosition.x = 1;
		}
		if (viewportPosition.x > 1) {
			viewportPosition.x = 0;
		}
		if (viewportPosition.y < 0) {
			viewportPosition.y = 1;
		}
		if (viewportPosition.y > 1) {
			viewportPosition.y = 0;
		}

		vectoredMovement.transform.position = Camera.main.ViewportToWorldPoint(viewportPosition);
	}
}