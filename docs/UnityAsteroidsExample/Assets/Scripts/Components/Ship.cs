using UnityEngine;
using RobotArms;

public class Ship : RobotArmsComponent {
	public float RotationSpeed;
	public float Fuel;
	public float MaxFuel;
	public float FuelConsumptionPerSecond;
	public float ThrustForce;
	public float MaxSpeed;
	public GameObject Projectile;
}