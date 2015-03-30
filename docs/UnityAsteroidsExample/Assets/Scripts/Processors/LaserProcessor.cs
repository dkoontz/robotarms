using UnityEngine;
using RobotArms;

namespace Emberscape {
	[ProcessorOptions(typeof(Projectile), typeof(Destroyable))]
	public class LaserProcessor : RobotArmsProcessor {
		
		public override void Process(GameObject entity) {
			var destroyable = entity.GetComponent<Destroyable>();

			if (destroyable.Destroyed) {
				RobotArmsUtils.RunAtEndOfFrame(() => GameObject.Destroy(entity));
			}
		}
	}
}