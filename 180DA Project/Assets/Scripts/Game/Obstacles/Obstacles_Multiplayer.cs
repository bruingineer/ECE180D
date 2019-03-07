using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacles_Multiplayer : Obstacles {
	private string subscribeTopic; 
	public static bool obstacleReady;
	public static int obstacleIndex;
	public static List<int> laserPositions;

	protected override void Awake() 
	{
		base.Awake();
		obstacleReady = false;
	}

	void Update () {
		HandleMultiplayer();
	}

	protected override void HandleChallengeReady()
	{
		if (obstacleReady)
			{
				Debug.Log("Starting Obstacle...");
				obstacleReady = false;
				StartChallenge();
			}
	}

	public override void SetUpClient(string header)
	{
		subscribeTopic = header + "obstacle";
		publishTopic = header + "request_obstacle";
		multiplayerClient = new ObstacleMultiplayerClient(subscribeTopic);
	}

	protected override IEnumerator HandleObstacles()
	{	
		yield return obstacles[obstacleIndex].StartObstacle(laserPositions);
		messageOut = false;
	}	
}