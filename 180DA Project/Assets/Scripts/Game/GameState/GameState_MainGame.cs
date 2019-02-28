using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameState_MainGame : GameState_with_Player {

	protected Player_Main player_MainGame;
	public List<GameObject> playerLives;
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
		startGame = true;
		gameMode = "main_game";
		player_MainGame = player.AddComponent<Player_Main>();
	}

	
	public void ChangePitch(int playerLives)
	{
		gameMusic.pitch = playerLives > 1 ? 1 : 1.25f;
	}

	public void RemoveLife(int curLives)
	{
		playerLives[curLives - 1].SetActive(false);
		ChangePitch(curLives);
	}

	protected override void HandleLose() 
	{
		if (player_MainGame.isDead) {
			gamePlaying = false;
			SelectedPlayer.died = true;
			result.text = "Game Over!";
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
