using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacles_Multiplayer : Obstacles {
	private string subscribeTopic; 
	ObstacleMultiplayerClient obstacleMultiplayerClient;
	private string publishTopic;
	private bool messageOut;
	public static bool obstacleReady;
	public static int obstacleIndex;
	public static List<int> laserPositions;

	protected override void SetUp() 
	{
		subscribeTopic = Multiplayer_Controller.playerHeader + "obstacle";
		publishTopic = Multiplayer_Controller.playerHeader + "request_obstacle";
		obstacleMultiplayerClient = new ObstacleMultiplayerClient(subscribeTopic);
		messageOut = false;
		obstacleReady = false;
	}

	void Update () {
		if (GameState_Base.gamePlaying)
		{
			if (!messageOut)
			{
				messageOut = true;
				obstacleMultiplayerClient.SendMessage(publishTopic, "requested");
				Debug.Log("Sending message to server!");
			}
			if (obstacleReady)
			{
				Debug.Log("Starting Obstacle...");
				obstacleReady = false;
				StartObstacles();
			}
		}
	}

	protected override IEnumerator HandleObstacles()
	{	
		yield return obstacles[obstacleIndex].StartObstacle(laserPositions);
		messageOut = false;
	}	
}