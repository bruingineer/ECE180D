using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser_MiniGame : MonoBehaviour {

	public GameObject bigLasers;
	public GameObject smallLasers;
	private List<GameObject> obstacles;
	public static bool obstacleOn;
	public static float obstacleWaitTime;
	public static bool playerWasHit;

	void Awake()
	{
		obstacleOn = false;
		obstacleWaitTime = 5f;
		obstacles = new List<GameObject> {bigLasers, smallLasers};
		playerWasHit = false;
	}
	
	void Update () {
		if(!obstacleOn && !Player.isDead && GameState.gamePlaying) {
			obstacleOn = true;
			GameObject laser_game = Instantiate(obstacles[Random.Range(0, obstacles.Count)]);
			StartCoroutine(HandlePlayerMove(laser_game));
		}
	}

	IEnumerator HandlePlayerMove(GameObject laser) 
	{
		while (laser != null) yield return null;
		if (!playerWasHit)
			PlayerMQTT_X.playerMoved = true;
		else 
			playerWasHit = false;
		yield return null;
	}
}
