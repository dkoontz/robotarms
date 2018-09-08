# README #

RobotArms is a Entity / Component / Processor\* library for [Unity3D](http://unity3d.com). RobotArms emphasizes usage of Unity components as pure data and moves the logic into one or more processors. This data oriented approach leverages Unity's editor to show the full state of your application with no hidden information inside objects. This approach is **not** object oriented but instead takes a more functional-style approach to data processing.

### Why? ###

Unity's current system does not easily allow for components that overlap in their concerns but have completely different implementations from coordinating. For example, if you have a component that moves a ship to a target point and you want to add another component that represents powerups for that ship, the powerup component would likely need to be able to effect the ship's speed. Without modifying your ship movement component it would be unlikely that the powerup component could do its job. However, if the ship's stats with regards to target point, speed, turn rate, etc. were simply data on a component, then any number of processors could read and or write to those fields. It would be easy to have the movement processor calculate the desired direction and default speed, and then for the powerup processor to modify that speed based on current powerups. Neither processor would have any knowledge of the other. If you find that you are often exposing data from a MonoBehaviour just so that *another* MonoBehaviour can read from it (or worse yet, write to it), RobotArms will be a breath of fresh air.


### More info ###

If you are unfamiliar with this type of system I'd suggest the following reading:

[Wikipedia](http://en.wikipedia.org/wiki/Entity_component_system)  
[Entity Systems are the future of MMOG development](http://t-machine.org/index.php/2007/09/03/entity-systems-are-the-future-of-mmog-development-part-1/)  
[Original 2002 GDC talk by Scott Bilas](http://scottbilas.com/files/2002/gdc_san_jose/game_objects_slides_with_notes.pdf)

\* Most literature on this topic uses the terms Entity, Component, and System. The usage of the term System has since fallen out of favor somewhat due to the overloaded nature of the term and frequent naming collisions that occur. RobotArms uses the term Processor, so when reading any articles on the topic just substitute "Processor" whenever you see "System".

We were originally inspired to write RobotArms after using the [Artemis](http://gamadu.com/artemis/) framework. We wanted something that integrated more directly with Unity, and thus RobotArms was born.

### Getting started ###

1. Get RobotArms
	1. Download the source and compile
	2. Copy the resulting dlls to your Unity project
		1. RobotArms.dll goes anywhere in you Assets folder
		2. RobotArmsEditor.dll goes into any folder named Editor in your Assets folder
	3. Alternatively just copy the RobotArms source files to your Unity Assets folder
		1. Remember to copy both the RobotArms source files and the RobotArmsEditor source files
2. Start writing Components and Processors
	1. See the tutorial below

### Basic Usage ###

The implementation of the steps outlined in this tutorial can be found in the docs/UnityAsteroidsExample folder.

To use RobotArms you will create one or more Components and then create one or more Processors to operate on those components. Let us consider the ship from a game like Asteroids and the data we would need.

* Ship
  * Position
  * Facing (rotation)
  * Velocity
  * Turn speed
  * Fuel
  * Fuel consumption rate
  * Thrust force
* Player Input
  * Keys/buttons for rotation
  * Key/button for thrusting forward

If you were writing this in regular Unity you would probably create a Ship MonoBehaviour that would take care of most of this. You might break these responsibilities into two parts, one for the vectored movement and the other for the ship specific bits if you thought that you might be able to reuse the vectored movement for other objects. It would be pretty tempting to check for input in the Ship MonoBehaviour as that's where you'd be applying thrust and possibly also moving the ship based on its velocity if you didn't opt for a separate vectored movement component. If you did go for 2 components then you'd likely either use GetComponent in your Ship MonoBehaviour to get the VectoredMovement component or you'd have a public field that you could drag/drop the component on to (or you could use [SerialiableField] like you **should** be doing to support a proper OOP style.)

In RobotArms you would likely want three separate components, one for the movement, one for the ship data, and one for the input. The position is taken care of thanks to Unity's Transform component. This leaves us with just Velocity to track for our movement component. For the ship we have rotation, rotation speed, current fuel (max fuel also if we can gain fuel), fuel consumption rate, and the thrust force. Facing can be retrieved from the Transform using something standard like the forward (z) vector as our facing. In 2d we'd have a similar concept but it would usually be the up (y) vector instead. For player input we could record the raw button presses or map that to something a bit more game specific. In this example I've chosen to go with a more game specific representation.

#### Components and their fields ####

Here are the components with broken out with the data each one will store.

* VectoredMovement
  - Velocity
* Ship
  - Turn speed
  - Fuel
  - Max fuel
  - Fuel consumption rate
  - Thrust force
* PlayerInput
  - Thrusting yes/no
  - Rotation input as a float representing how much to rotate
    + -1 = max counterclockwise
    + 0 = no rotation
    + 1 = max clockwise

#### Component implementation ####

```cs
using UnityEngine;
using RobotArms;

public class VectoredMovement : RobotArmsComponent {
    public Vector3 Velocity;
}
```

```cs
using UnityEngine;
using RobotArms;

public class Ship : RobotArmsComponent {
	public float RotationSpeed;
	public float Fuel;
	public float MaxFuel;
	public float FuelConsumptionPerSecond;
	public float ThrustForce;
}
```

```cs
using UnityEngine;
using RobotArms;

public class PlayerInput : RobotArmsComponent {
	public bool Thrust;
	public float Rotation;
}
```

RobotArmsComponent is just a subclass of MonoBehaviour that does all the registering of the component with the system for you. Components by themselves don't do anything, so now we can add in a processor to start adding functionality to our game.

#### Processors ####

The simplest element of this game is probably the vectored movement. We simply translate the GameObject based on the velocity vector. This sounds like a VectoredMovementProcessor to me.

```cs
using UnityEngine;
using RobotArms;

public class VectoredMovementProcessor : RobotArmsProcessor<VectoredMovement> {
	public override void Process (GameObject entity, VectoredMovement movement) {
		entity.transform.Translate(movement.Velocity * Time.deltaTime, Space.World);
	}
}
```

Processors declare which GameObjects (aka entities) they wish to receive via the ProcessorOptions attribute. ProcessorOptions is how a processor says "I'm only interested in GameObjects that have the following component(s) attached". ProcessorOptions also allows you to change when a processor is run (Update / FixedUpdate / LateUpdate), as well as specifying a priority so that you can force certain processors to run earlier or later than the rest just like how script execution order works in Unity. Next you need to override either the Process or ProcessAll methods.

The Process method is handed one entity at a time while ProcessAll is handed an IEnumerable<GameObject> of all the entities that match the processor's criteria. This second type is useful when you need to make a decision that cuts across all of a certain kind of thing, for example targeting the highest threat enemy. Inside the Process/ProcessAll method you will need to retrieve the component(s) that you want to work with via Unity's GetComponent method. We can rest assured that entity WILL have a VectoredMovement component attached, otherwise it would not have been passed into this processor.

Now we don't have a complete system but we do have enough to start testing things out. To get RobotArms working in a scene you'll need 2 things. Create a GameObject to be the ship and attach the VectoredMovement component. Then, create a new empty GameObject and attach the RobotArmsCoordinator component. When you press play nothing will happen unless you have set the values on the Velocity field of the VectoredMovement component to non-zero. Go ahead and adjust the values and you will see the ship begin to move.

Next let's write a processor that reads input and sets the values on the PlayerInput component.

```cs
using UnityEngine;
using RobotArms;

public class PlayerInputProcessor : RobotArmsProcessor<PlayerInput> {
	public override void Process (GameObject entity, PlayerInput input) {
		input.Thrust = Input.GetAxis("Vertical") > 0;
		input.Rotation = Input.GetAxis("Horizontal");
	}
}
```

Go ahead and attach the PlayerInput component to your ship GameObject and run the game. If you press the correct inputs (W/A/D, Arrow Keys, Analog Stick on a Gamepad) you should now see the values updating on the component. The last step is now to write a ShipProcessor that uses the data from the Ship and PlayerInput components to update the VectoredMovement component. This is an important difference from how you are probably used to doing things in Unity. The PlayerInputProcessor does not know how the information it is generating will be used, and the ShipProcessor does not know where the information it is using came from. All the places you would normally have one component call a method on another component are instead replaced with reading from and writing to components. This means all the state of your application is right there in your components and you can pause Unity, click around and see exactly what's going on. The first time you have a puzzling error in your application and you simply pause Unity, go find the offending component, see one or more values that are clearly incorrect and make an easy fix, you will be sold on this idea. We have gone to great lengths to ensure that all of a RobotArms application's data can be inspected via the Unity editor. Alright, on to the final processor, time to bring this all together!

```cs
using UnityEngine;
using RobotArms;

public class ShipMovementProcessor : RobotArmsProcessor<Ship, PlayerInput, VectoredMovement> {
	public override void Process (GameObject entity, Ship ship, PlayerInput input, VectoredMovement movement) {
		if (ship.Fuel > 0) {
			if (input.Thrust) {
				movement.Velocity += entity.transform.up * ship.ThrustForce * Time.deltaTime;
				ship.Fuel -= ship.FuelConsumptionPerSecond * Time.deltaTime;
			}

			entity.transform.Rotate(0, 0, -input.Rotation * ship.RotationSpeed * Time.deltaTime);
		}
	}
}
```
Here we have an example of a processor that requires 3 different components. Only a GameObject that has all three of these components will be passed in to this processor, so asteroids floating around with VectoredMovement components will never show up here, nor will enemy ships with Ship and VectoredMovement components but not a PlayerInput component. Add the Ship component to your ship GameObject, set some values for the rotation speed, fuel, fuel consumption rate, and thrust force and hit play.

#### Ship UI ####

Currently the player has no way of knowing how much fuel they have left or when their thruster is active. To rectify this we can display one or more graphics on the screen to represent the remaining fuel and and effect for thrusting. For the fuel display I chose to use 10 sprites, each representing 10% of the ship's fuel supply. For the thrusting effect I simply positioned a sprite that can be turned on and off according to the player's input. After positioning the sprites on-screen we need some way to reference them.

```cs
using UnityEngine;
using RobotArms;

public class ShipDisplay : RobotArmsComponent {
	public SpriteRenderer[] FuelIcons;
	public SpriteRenderer Thruster;
}
```
Attach the ShipDisplay component to your ship and add in whatever graphics you wish for the fuel and thruster.

Next we'll need to modify these sprites based on the state of the ship, sounds like a ShipDisplayProcessor to me. To make the usage of fuel feel a little more immediate I chose to scale down the pips as you consume fuel.

```cs
using UnityEngine;
using RobotArms;

public class ShipDisplayProcessor : RobotArmsProcessor<Ship, ShipDisplay, PlayerInput> {
	public override void Process (GameObject entity, Ship ship, ShipDisplay display, PlayerInput input) {
		var fuelPercent = Mathf.Clamp(ship.Fuel / ship.MaxFuel, 0, 1);
		var fuelPipsToShow = Mathf.FloorToInt(fuelPercent * display.FuelIcons.Length);
		var fuelPerPip = ship.MaxFuel / display.FuelIcons.Length;
		for (var i = 0; i < display.FuelIcons.Length; ++i) {
			display.FuelIcons[i].renderer.enabled = i <= fuelPipsToShow;

			if (fuelPipsToShow == i) {
				display.FuelIcons[fuelPipsToShow].transform.localScale = new Vector3(1, ship.Fuel % fuelPerPip, 1);
			}
			else {
				display.FuelIcons[i].transform.localScale = Vector3.one;
			}
		}

		display.Thruster.renderer.enabled = input.Thrust;
	}
}
```

This processor is certainly the most complex, but as you probably know, display logic is often the messiest part of an app. What's great about the RobotArms approach is there's very little temptation to mix the logic in ShipProcessor with ShipDisplayProcessor.

So there we go. It's not a huge project but hopefully this has given you a taste of how RobotArms is different and perhaps how it can make your Unity development more productive.

### Caveats ###

1. We are using Linq and foreach. If this aggravates your hyper-optimizing self and you want to rewrite everything to have absolutely zero memory allocation, patches are accepted. RobotArms has shown to be very trivial in our profiling, even on mobile.

### License ###

This project is licensed under The MIT License
Copyright 2014 David Koontz, Trenton Kennedy

Please direct questions, patches, and suggestions to the project page at
https://bitbucket.org/dkoontz/robotarms

### External Projects ###

The demo project uses sprites provided by Kenney Vleugels (www.kenney.nl)  
http://opengameart.org/content/space-shooter-redux

### Contact ###

Bugs, comments, complaints should be directed to the [RobotArms repository](https://bitbucket.org/dkoontz/robotarms)

Or contact the authors directly:  
David Koontz - david@koontzfamily.org  
Trenton Kennedy - trentonkennedy@gmail.com
