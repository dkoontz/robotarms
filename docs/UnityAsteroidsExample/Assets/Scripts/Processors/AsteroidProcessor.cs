using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using RobotArms;
using UnityEngine.UI;

public class AsteroidProcessor : RobotArmsProcessor<Asteroid, VectoredMovement, Destroyable> {

	public override void Initialize(GameObject entity, Asteroid asteroid, VectoredMovement vectoredMovement, Destroyable destroyable) {
		SetSpriteAndBoundsForCurrentSize(asteroid);
		RandomizeAsteroidMovement(vectoredMovement);
	}

	public override void Process(GameObject entity, Asteroid asteroid, VectoredMovement vectoredMovement, Destroyable destroyable) {
		var asteroidRoot = GetBlackboard<Blackboard>().AsteroidRoot;

		if (destroyable.Destroyed) {
			destroyable.Destroyed = false;
			switch (asteroid.Size) {
				case AsteroidSize.Large:
					asteroid.Size = AsteroidSize.Medium;
					BreakAsteroid(asteroid, vectoredMovement, asteroidRoot);
					break;
				case AsteroidSize.Medium:
					asteroid.Size = AsteroidSize.Small;
					BreakAsteroid(asteroid, vectoredMovement, asteroidRoot);
					break;
				case AsteroidSize.Small:
					RobotArmsUtils.RunAtEndOfFrame(() => {
						GameObject.Destroy(asteroid.gameObject);
					});
					break;
			}

			GameObject.Instantiate(destroyable.DestroyedEffect, entity.transform.position, entity.transform.rotation);
		}
	}

	static void BreakAsteroid(Asteroid asteroid, VectoredMovement vectoredMovement, Transform asteroidRoot) {
		// New asteroid will be initialized in the above Initialize method as it is a new GameObject with new components.
		RobotArmsUtils.RunAtEndOfFrame(() => {
			var newAsteroid = GameObject.Instantiate(asteroid.gameObject) as GameObject;
			newAsteroid.transform.parent = asteroidRoot;
			
			RandomizeAsteroidMovement(vectoredMovement);
			SetSpriteAndBoundsForCurrentSize(asteroid);
		});
	}

	static void RandomizeAsteroidMovement(VectoredMovement vectoredMovement) {
		vectoredMovement.Velocity = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f));
		vectoredMovement.Rotation = Random.Range(-4f, 4f);
	}

	static void SetSpriteAndBoundsForCurrentSize(Asteroid asteroid) {
		var renderer = asteroid.GetComponent<SpriteRenderer>();
		renderer.sprite = GetRandomSprite(asteroid);
		var collider = asteroid.GetComponent<BoxCollider2D>();
		collider.size = renderer.bounds.size;
		collider.offset = Vector2.zero;

	}

	static Sprite GetRandomSprite(Asteroid asteroid) {
		switch (asteroid.Size) {
			case AsteroidSize.Large:
				return asteroid.LargeAsteroids[Random.Range(0, asteroid.LargeAsteroids.Length)];
			case AsteroidSize.Medium:
				return asteroid.MediumAsteroids[Random.Range(0, asteroid.MediumAsteroids.Length)];
			case AsteroidSize.Small:
				return asteroid.SmallAsteroids[Random.Range(0, asteroid.SmallAsteroids.Length)];
			default:
				throw new System.ArgumentException("Unknown asteroid size: " + asteroid.Size);
		}
	}
}