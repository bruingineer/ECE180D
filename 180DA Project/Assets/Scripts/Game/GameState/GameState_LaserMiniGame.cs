using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameState_LaserMiniGame : GameState_with_Player
 {
	public List<GameObject> playerLives;
	Player_Main player_MainGame;
	// add title with selected difficulty
	protected override void SetUp_Events_Obstacles()
	{
		gameObject.AddComponent<Obstacles_Laser_Minigame>().StartObstacles();
	}

	protected override void HandlePostGameScene()
	{
		SetUpButtons();
	}

	protected override void SetUp()
	{
		base.SetUp();
		player_MainGame = player.AddComponent<Player_Main>();
		gameMode = "laser_training";
	}

}
