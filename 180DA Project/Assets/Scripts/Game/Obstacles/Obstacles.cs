using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Obstacles : Challenge {

	protected Laser_Obstacle bigLaserObstacle, smallLaserObstacle;
	protected List<Laser_Obstacle> obstacles;
	private bool handledCleanup = false;

	protected override void Awake()
	{
		bigLaserObstacle = gameObject.AddComponent<Big_Laser_Obstacle>();
		smallLaserObstacle = gameObject.AddComponent<Small_Laser_Obstacle>();
		obstacles = new List<Laser_Obstacle>{smallLaserObstacle, bigLaserObstacle};

	}
	
	protected abstract IEnumerator HandleObstacles();

	public override void StartChallenge()
	{
		StartCoroutine(HandleObstacles());
	}


}
