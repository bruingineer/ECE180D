using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameState_LaserMiniGame : GameState_with_Lives
 {
	// add title with selected difficulty
	protected GestureClient gestureClient;
	protected string openPoseTopic = "openpose/change";
	protected override void SetUp_Events_Obstacles()
	{
		challenges.Add(gameObject.AddComponent<Obstacles_Laser_Minigame>());
		base.SetUp_Events_Obstacles();
	}

	protected override void HandlePostGameScene()
	{
		SetUpButtons();
	}

	protected override void Awake()
	{
		gestureClient = new GestureClient(openPoseTopic);
		gestureClient.SendMessage(openPoseTopic, "single_player");
		gameMode = "laser_training";
		base.Awake();
	}

}
