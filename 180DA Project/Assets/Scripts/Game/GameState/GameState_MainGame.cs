using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameState_MainGame : GameState_with_Lives {

	protected Player_Main player_MainGame;

	protected override void SetUp_Events_Obstacles()
	{
		gameObject.AddComponent<Obstacles_Main>().StartObstacles();
		gameObject.AddComponent<PlayerEvents_MainGame>().StartEvents();
	}

	protected override void HandlePostGameScene()
	{
		LoadStatsMenu();
	}

	protected override void Awake()
	{
		base.Awake();
		gameMode = "main_game";
		player_MainGame = player.AddComponent<Player_Main>();
	}

}
