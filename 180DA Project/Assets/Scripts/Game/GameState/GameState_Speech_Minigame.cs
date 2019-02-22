using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameState_Speech_Minigame : GameState_Event_Minigame
 {
	 // add title with selected difficulty
	protected override void SetUp_Events_Obstacles()
	{
		gameObject.AddComponent<PlayerEvents_Speech_Minigame>().StartEvents();
	}

	protected override void HandlePostGameScene()
	{
		SetUpButtons();
	}

}
