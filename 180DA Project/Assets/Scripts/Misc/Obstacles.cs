using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacles : Moving_Object {

	public Obstacle bigLasers, smallLasers;
	private List<Obstacle> obstacles;
	private Obstacle curObstacle;

	void Awake()
	{
		obstacles = new List<Obstacle> {bigLasers, smallLasers};
	}
	
	void Update () {
		if(!curObstacle && !Player.isDead && GameState.gamePlaying) {
			curObstacle = Instantiate(obstacles[Random.Range(0, obstacles.Count)]);
		}
		// fix this by having obstacles be created after the timer
		// CheckGamePlaying(); TODO
	}
}
