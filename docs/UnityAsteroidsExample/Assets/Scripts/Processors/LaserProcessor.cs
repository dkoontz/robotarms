using UnityEngine;
using RobotArms;

namespace Emberscape {
	public class LaserProcessor : RobotArmsProcessor<Projectile, Destroyable> {

		public override void Process(GameObject entity, Projectile projectile, Destroyable destroyable) {
			if (destroyable.Destroyed) {
				RobotArmsUtils.RunAtEndOfFrame(() => GameObject.Destroy(entity));
			}
		}
	}
}