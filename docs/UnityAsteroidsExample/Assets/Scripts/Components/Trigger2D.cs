using System.Collections.Generic;
using UnityEngine;
using RobotArms;

public class Trigger2D : RobotArmsComponent {

	public List<Collider2D> Colliders = new List<Collider2D>();

	public void OnTriggerEnter2D(Collider2D other) {
		Colliders.Add(other);
	}

	public void OnTriggerExit2D(Collider2D other) {
		Colliders.Remove(other);
	}
}