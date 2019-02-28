using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameState_MainGame : GameState_with_Player {

	protected Player_Main player_MainGame;

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
		player_MainGame = player.AddComponent<Player_Main>();
		StartGame();
	}

	protected override void HandleLose() 
	{
		if (player_MainGame.isDead) {
			gamePlaying = false;
			SelectedPlayer.died = true;
			result.text = "Game Over!";
			// add for handle lose for multiplayer
			ParticleSystem explosion = Instantiate(playerExplosion, player.transform.position, Quaternion.identity).GetComponent<ParticleSystem>();
			Destroy(player.gameObject);
			PlayClip(gameLostMusic);
			float explosionDuration = gameLostMusic.length;
			var main = explosion.main;
			main.duration = explosionDuration;
			explosion.Play();
			StartCoroutine(HandlePostGame(explosionDuration));
		}
	}

	
}
