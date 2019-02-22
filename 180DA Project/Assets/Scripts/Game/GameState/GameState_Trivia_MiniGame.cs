using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameState_Trivia_MiniGame : GameState_Event_Minigame {

	 // add title with selected difficulty
	protected override void SetUp_Events_Obstacles()
	{
		gameObject.AddComponent<PlayerEvents_Trivia_Minigame>().StartEvents();
	}

	protected override void SetUp()
	{
		base.SetUp();
		gameMode = "trivia_training";
	}
}
