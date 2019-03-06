using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameState_GestureMinigame : GameState_Event_Minigame {

	 // add title with selected difficulty
	protected override void SetUp_Events_Obstacles()
	{
		gameObject.AddComponent<PlayerEvents_Gesture_Minigame>().StartEvents();
	}

	protected override void Awake()
	{
		base.Awake();
		gameMode = "gesture_training";
	}
}
