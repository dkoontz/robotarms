using UnityEngine;
using RobotArms;

public enum AsteroidSize {
	Large, Medium, Small
}

[RequireComponent(typeof(VectoredMovement), typeof(Destroyable))]
public class Asteroid : RobotArmsComponent {
	public Sprite[] LargeAsteroids;
	public Sprite[] MediumAsteroids;
	public Sprite[] SmallAsteroids;

	public AsteroidSize Size;
}