using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacles : MonoBehaviour {

	public GameObject bigLasers;
	private List<GameObject> obstacles;
	public static bool obstacleOn;
	public static float obstacleWaitTime;

	void Awake()
	{
		obstacleOn = false;
		obstacleWaitTime = 3f;
		obstacles = new List<GameObject> {bigLasers};
	}
	
	void Update () {
		if(!obstacleOn && !Player.isDead) {
			obstacleOn = true;
			Instantiate(obstacles[Random.Range(0, obstacles.Count)]);
		}
	}
}
