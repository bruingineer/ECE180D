using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameState_LaserMiniGame : GameState_with_Lives
 {
	// add title with selected difficulty
	protected override void SetUp_Events_Obstacles()
	{
		gameObject.AddComponent<Obstacles_Laser_Minigame>().StartObstacles();
	}

	protected override void HandlePostGameScene()
	{
		SetUpButtons();
	}

	protected override void Awake()
	{
		gameMode = "laser_training";
		base.Awake();
	}

}
