using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacles : MonoBehaviour {

	protected Laser_Obstacle bigLaserObstacle, smallLaserObstacle;
	protected List<Laser_Obstacle> obstacles;

	protected virtual void Awake()
	{
		bigLaserObstacle = gameObject.AddComponent<Big_Laser_Obstacle>();
		smallLaserObstacle = gameObject.AddComponent<Small_Laser_Obstacle>();
		obstacles = new List<Laser_Obstacle>{smallLaserObstacle, bigLaserObstacle};
	}

	public void StartObstacles()
	{
		StartCoroutine(HandleObstacles());
	}

	private IEnumerator HandleObstacles()
	{	
		while(GameState_Base.gamePlaying)
		{	
			yield return obstacles[Random.Range(0, obstacles.Count)].StartObstacle();
			Handle_Minigame();
		}
	}
	protected virtual void Handle_Minigame() {}
}

	// for multiplayer
	// in update loop
	// if gamestateplaying
		// if !message out
			// message out = true
			// use client to request message
		// if obstacle ready
			// obstacle ready = false
			// call coroutine that yield returns until obstacle ends
			// in same function set message out to false
