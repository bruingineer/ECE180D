using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameState_Trivia_MiniGame : GameState_Event_Minigame {

	 // add title with selected difficulty
	protected override void SetUp_Events_Obstacles()
	{
		challenges.Add(gameObject.AddComponent<PlayerEvents_Trivia_Minigame>());
		base.SetUp_Events_Obstacles();
	}

	protected override void Awake()
	{
		base.Awake();
		gameMode = "trivia_training";
	}
}
