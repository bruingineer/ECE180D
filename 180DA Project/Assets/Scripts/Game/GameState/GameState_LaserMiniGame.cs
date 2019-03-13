using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameState_LaserMiniGame : GameState_with_Lives
 {
	// add title with selected difficulty
	protected override void SetUp_Events_Obstacles()
	{
		challenges.Add(gameObject.AddComponent<Obstacles_Laser_Minigame>());
		base.SetUp_Events_Obstacles();
	}

	protected override void HandlePostGameScene()
	{
		SetUpButtons();
	}

	protected override void Awake()
	{
		end_row = 13;
		gameMode = "laser_training";
		base.Awake();
	}

}
