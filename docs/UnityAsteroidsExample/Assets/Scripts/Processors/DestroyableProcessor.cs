using System.Linq;
using UnityEngine;
using RobotArms;

public class DestroyableProcessor : RobotArmsProcessor<Destroyable, Trigger2D> {
	
	public override void Process(GameObject entity, Destroyable destroyable, Trigger2D trigger) {
		if (trigger.Colliders.Where(c => c != null).Any(c => destroyable.DestroyWhenTouchingTag.Contains(c.gameObject.tag))) {
			destroyable.Destroyed = true;
		}
	}
}