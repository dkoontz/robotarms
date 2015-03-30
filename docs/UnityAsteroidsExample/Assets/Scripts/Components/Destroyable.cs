using UnityEngine;
using RobotArms;

public class Destroyable : RobotArmsComponent {
	public string[] DestroyWhenTouchingTag;
	public bool Destroyed;
	public GameObject DestroyedEffect;
}