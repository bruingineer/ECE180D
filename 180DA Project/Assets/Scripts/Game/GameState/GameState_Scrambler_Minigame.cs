﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameState_Scrambler_Minigame : GameState_Event_Minigame
 {
	 // add title with selected difficulty
	protected override void SetUp_Events_Obstacles()
	{
		// change to pass in gameobject and add to that component
		challenges.Add(gameObject.AddComponent<PlayerEvents_Scrambler_Minigame>());
		base.SetUp_Events_Obstacles();
	}

	protected override void Awake()
	{
		base.Awake();
		gameMode = "unscramble_training";
	}
}
