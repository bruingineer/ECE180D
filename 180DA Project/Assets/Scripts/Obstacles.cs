using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacles : MonoBehaviour {

	public GameObject bigLasers;
	private List<GameObject> obstacles;
	public static bool obstacleOn = false;
	private int obstacleCount;
	public static float obstacleWaitTime;

	void Awake()
	{
		obstacleWaitTime = 3f;
		obstacles = new List<GameObject> {bigLasers};
		obstacleCount = obstacles.Count;
	}
	
	// Update is called once per frame
	void Update () {
		if(!obstacleOn && !Player.gameOver) {
			int index = Random.Range(0, obstacleCount);
			obstacleOn = true;
			Instantiate(obstacles[index]);
		}
	}
}
