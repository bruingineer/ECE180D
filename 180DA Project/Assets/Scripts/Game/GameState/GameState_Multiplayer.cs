using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameState_Multiplayer : GameState_with_Player {

	protected override void SetUp_Events_Obstacles()
	{
		Debug.Log("Setting up obstacles and events");
	}

	protected override void HandlePostGameScene()
	{
		Debug.Log("Handling Post Game");
	}

	protected override void SetUp()
	{
		base.SetUp();
		gameMode = "multiplayer_game";
	}

	protected override void HandleCountdown()
	{
		countdown.gameObject.SetActive(false);
	}
}
