using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Obstacles : MonoBehaviour {

	protected Laser_Obstacle bigLaserObstacle, smallLaserObstacle;
	protected List<Laser_Obstacle> obstacles;
	private bool handledCleanup = false;

	protected virtual void Awake()
	{
		bigLaserObstacle = gameObject.AddComponent<Big_Laser_Obstacle>();
		smallLaserObstacle = gameObject.AddComponent<Small_Laser_Obstacle>();
		obstacles = new List<Laser_Obstacle>{smallLaserObstacle, bigLaserObstacle};

	}
	
	protected abstract IEnumerator HandleObstacles();

	public void StartObstacles()
	{
		StartCoroutine(HandleObstacles());
	}


}
