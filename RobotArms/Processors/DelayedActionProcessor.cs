using UnityEngine;
using System.Collections;
using RobotArms;

namespace RobotArms.BuiltIn {
	[ProcessorOptions(typeof(DelayedAction))]
	public class DelayedActionProcessor : RobotArmsProcessor {

		public override void Process(GameObject entity) {
			var delayedActions = entity.GetComponents<DelayedAction>();

			foreach (var delayedAction in delayedActions) {
				
				if (!delayedAction.Running) {
					return;
				}
				
				delayedAction.Delay -= Time.deltaTime;
				
				if (delayedAction.Delay > 0) {
					if (delayedAction.OnUpdate != null) {
						delayedAction.OnUpdate();
					}
				}
				else {
					if (delayedAction.OnComplete != null) {
						delayedAction.OnComplete();
					}
					DestroyComponent(delayedAction);
				}
			}
		}
	}
}