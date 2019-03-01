using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Obstacles : MonoBehaviour {

	protected Laser_Obstacle bigLaserObstacle, smallLaserObstacle;
	protected List<Laser_Obstacle> obstacles;

	protected virtual void Awake()
	{
		bigLaserObstacle = gameObject.AddComponent<Big_Laser_Obstacle>();
		smallLaserObstacle = gameObject.AddComponent<Small_Laser_Obstacle>();
		obstacles = new List<Laser_Obstacle>{smallLaserObstacle, bigLaserObstacle};
		SetUp();
	}

	// change to base awake function
	protected virtual void SetUp() {}

	protected abstract IEnumerator HandleObstacles();

	public void StartObstacles()
	{
		StartCoroutine(HandleObstacles());
	}

}
