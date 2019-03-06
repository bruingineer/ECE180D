using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameState_Multiplayer : GameState_with_Player {

	protected override void SetUp_Events_Obstacles()
	{
		gameObject.AddComponent<Obstacles_Multiplayer>();
		gameObject.AddComponent<PlayerEvents_Multiplayer>();
	}

	protected override void HandlePostGameScene()
	{
		SetUpButtons();
	}

	protected override void HandleLose()
	{
		
	}

	protected override void GameWon()
	{
		base.GameWon();
	 	Multiplayer_Controller.multiplayerClient.SendMessage(Multiplayer_Controller.playerConnectedTopic, Multiplayer_Controller.wonMessage);
	}

	protected override void Awake()
	{
		base.Awake();
		gameMode = "multiplayer_game";
	}
}
