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
		obstacleWaitTime = 5f;
		obstacles = new List<GameObject> {bigLasers};
		obstacleCount = obstacles.Count;
	}
	
	// Update is called once per frame
	void Update () {
		if(!obstacleOn) {
			int index = Random.Range(0, obstacleCount);
			Instantiate(obstacles[index]);
		}
	}
}
