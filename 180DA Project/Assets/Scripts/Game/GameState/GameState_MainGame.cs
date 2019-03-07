using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameState_MainGame : GameState_with_Lives {

	protected Player_Main player_MainGame;
	protected override void SetUp_Events_Obstacles()
	{
		challenges.Add(gameObject.AddComponent<Obstacles_Main>());
		challenges.Add(gameObject.AddComponent<PlayerEvents_MainGame>());
		base.SetUp_Events_Obstacles();
	}

	protected override void HandlePostGameScene()
	{
		LoadStatsMenu();
	}

	protected override void Awake()
	{
		gameMode = "main_game";
		base.Awake();
		player_MainGame = player.AddComponent<Player_Main>();
		
	}

}
