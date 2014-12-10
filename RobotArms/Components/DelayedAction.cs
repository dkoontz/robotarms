using System;
using RobotArms;

namespace RobotArms.BuiltIn {
	public class DelayedAction : RobotArmsComponent {

		public float Delay;
		public bool Running = true;
		public Action OnUpdate;
		public Action OnComplete;
	}
}