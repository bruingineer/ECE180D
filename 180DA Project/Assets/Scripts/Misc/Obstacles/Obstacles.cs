using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacles : Moving_Object {

	protected Obstacle bigLaserObstacle, smallLaserObstacle;
	protected List<Obstacle> obstacles;

	protected virtual void Awake()
	{
		bigLaserObstacle = gameObject.AddComponent<Big_Laser_Obstacle>();
		smallLaserObstacle = gameObject.AddComponent<Small_Laser_Obstacle>();
		obstacles = new List<Obstacle>{bigLaserObstacle, smallLaserObstacle};
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
