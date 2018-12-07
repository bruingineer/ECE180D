using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacles : MonoBehaviour {

	public GameObject bigLasers;
	public GameObject smallLasers;
	private List<GameObject> obstacles;
	public static bool obstacleOn;
	public static float obstacleWaitTime;

	void Awake()
	{
		obstacleOn = false;
		obstacleWaitTime = 5f;
		obstacles = new List<GameObject> {bigLasers, smallLasers};
	}
	
	void Update () {
		if(!obstacleOn && !Player.isDead && GameState.gamePlaying) {
			obstacleOn = true;
			Instantiate(obstacles[Random.Range(0, obstacles.Count)]);
		}
		if (GameState.gameWon) 
			Destroy(gameObject);
	}
}
