using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameState_MainGame : GameState_with_Player {

	protected override void SetUp_Events_Obstacles()
	{
		gameObject.AddComponent<Obstacles>().StartObstacles();
		gameObject.AddComponent<PlayerEvents_MainGame>().StartEvents();
	}

	protected override void HandlePostGameScene()
	{
		LoadStatsMenu();
	}

	protected override void SetUp()
	{
		base.SetUp();
		gameMode = "main_game";
	}
}
