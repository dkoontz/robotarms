using System.Linq;
using UnityEngine;
using RobotArms;

[ProcessorOptions(typeof(Destroyable), typeof(Trigger2D))]
public class DestroyableProcessor : RobotArmsProcessor {
	
	public override void Process(GameObject entity) {
		var destroyable = entity.GetComponent<Destroyable>();
		var trigger = entity.GetComponent<Trigger2D>();

		if (trigger.Colliders.Where(c => c != null).Any(c => destroyable.DestroyWhenTouchingTag.Contains(c.gameObject.tag))) {
			destroyable.Destroyed = true;
		}
	}
}