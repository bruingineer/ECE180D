using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacles : Moving_Object {

	Obstacle bigLaserObstacle, smallLaserObstacle;
	private List<Obstacle> obstacles;
	private bool curObstacle = false;

	void Awake()
	{
		bigLaserObstacle = gameObject.AddComponent<Big_Laser_Obstacle>();
		smallLaserObstacle = gameObject.AddComponent<Small_Laser_Obstacle>();
		obstacles = new List<Obstacle> {bigLaserObstacle, smallLaserObstacle};
	}
	
	void Update () {
		if(!curObstacle && !Player.isDead && GameState.gamePlaying) {
			curObstacle = true;
			smallLaserObstacle.StartObstacle();
			// curObstacle = Instantiate(obstacles[Random.Range(0, obstacles.Count)]);
		}
		// fix this by having obstacles be created after the timer
		// CheckGamePlaying(); TODO
	}
}
