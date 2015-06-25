using UnityEngine;
using RobotArms;

public class ShipDisplayProcessor : RobotArmsProcessor<Ship, ShipDisplay, PlayerInput> {
	
	public override void Process (GameObject entity, Ship ship, ShipDisplay display, PlayerInput input) {
		var fuelPercent = Mathf.Clamp(ship.Fuel / ship.MaxFuel, 0, 1);
		var fuelPipsToShow = Mathf.FloorToInt(fuelPercent * display.FuelIcons.Length);
		var fuelPerPip = ship.MaxFuel / display.FuelIcons.Length;
		for (var i = 0; i < display.FuelIcons.Length; ++i) {
			display.FuelIcons[i].GetComponent<Renderer>().enabled = i <= fuelPipsToShow;

			if (fuelPipsToShow == i) {
				display.FuelIcons[fuelPipsToShow].transform.localScale = new Vector3(1, ship.Fuel % fuelPerPip, 1);
			}
			else {
				display.FuelIcons[i].transform.localScale = Vector3.one;
			}
		}

		display.Thruster.GetComponent<Renderer>().enabled = input.Thrust;
	}
}