using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameState_Multiplayer : GameState_with_Player {
	private bool loseHandled = false;
	private bool winHandled = false;
	protected override void SetUp_Events_Obstacles()
	{
		gameObject.AddComponent<Obstacles_Multiplayer>();
		gameObject.AddComponent<PlayerEvents_Multiplayer>();
	}

	protected override void HandlePostGameScene()
	{
		SetUpButtons();
	}

	protected override void Update()
	{
		base.Update();
		if (Multiplayer_Controller.won && !winHandled)
		{
			winHandled = true;
			base.GameWon();
		}
	}

	protected override void HandleLose()
	{
		if (Multiplayer_Controller.lost && !loseHandled)
		{
			loseHandled = true;
			result.text = "You Lose!";
			DestroyPlayer();
		}
	}

	protected override void GameWon()
	{
	 	Multiplayer_Controller.multiplayerClient.SendMessage(Multiplayer_Controller.playerConnectionTopic, Multiplayer_Controller.wonMessage);
	}

	protected override void Awake()
	{
		base.Awake();
		gameMode = "multiplayer_game";
	}
}
