using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacles : Moving_Object {

	Obstacle bigLaserObstacle, smallLaserObstacle;
	private List<Obstacle> obstacles;

	void Awake()
	{
		bigLaserObstacle = gameObject.AddComponent<Big_Laser_Obstacle>();
		smallLaserObstacle = gameObject.AddComponent<Small_Laser_Obstacle>();
		obstacles = new List<Obstacle> {bigLaserObstacle, smallLaserObstacle};
	}

	void Start()
	{
		StartCoroutine(HandleObstacles());
	}

	private IEnumerator HandleObstacles()
	{	
		// change to game playing
		while(true)
		{	
			yield return obstacles[Random.Range(0, obstacles.Count-1)].StartObstacle();
		}
		// yield return null;
	}
}
