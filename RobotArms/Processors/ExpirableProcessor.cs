using UnityEngine;
using System.Collections;
using RobotArms;

namespace RobotArms.BuiltIn {
	[ProcessorOptions(typeof(Expirable))]
	public class ExpirableProcessor : RobotArmsProcessor {

		public override void Process(GameObject entity) {
			var expirable = entity.GetComponent<Expirable>();
			expirable.TimeRemaining -= Time.deltaTime;

			if (expirable.TimeRemaining <= 0) {
				DestroyGameObject(expirable.Target);
			}
		}
	}
}