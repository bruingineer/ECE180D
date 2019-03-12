using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class GameState_Multiplayer : GameState_with_Player {
	private bool loseHandled = false;
	private bool winHandled = false;
	private bool gameWonHandled = false;
	protected override void SetUp_Events_Obstacles()
	{
		gameObject.AddComponent<Obstacles_Multiplayer>();
		gameObject.AddComponent<PlayerEvents_Multiplayer>();
	}

	protected override void HandlePostGameScene()
	{
		SetUpButtons();
	}

	public override void RetryLevel()
	{
		DisableButtons();
		GetComponent<Multiplayer_Controller>().ResetReadyButton();
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

	protected override void HandleCountdown()
	{
		countdown.gameObject.SetActive(false);
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

	protected override void HandlePlayerDestroyed()
	{
		player.SetActive(false);
	}

	protected override void GameWon()
	{
		if (!gameWonHandled)
		{
	 		Multiplayer_Controller.multiplayerClient.SendMessage(Multiplayer_Controller.playerConnectionTopic, Multiplayer_Controller.wonMessage);
			gameWonHandled = true;
		}
	}

	public void ResetGameState()
	{
		player.SetActive(true);
		player.transform.position = playerStartPosition;
		countdown.text = "3";
		gameWonHandled = false;
		loseHandled = false;
		winHandled = false;
	}

	protected override void Awake()
	{
		
		gameMode = "multiplayer_game";
		base.Awake();
	}

	protected override void SetUpDifficulty()
	{
		gameDifficulty = "easy";
	}

	protected override void AddComponentToPlayer()
    {
        player.AddComponent<Player_Multiplayer>();
    }
}
