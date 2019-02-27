using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEvents_MainGame : PlayerEvents {

	protected Event gestureGame, scramblerGame, triviaGame;
	public override void Awake()
	{
		gestureGame = gameObject.AddComponent<GestureGame>();
		scramblerGame = gameObject.AddComponent<WordScramble>();
		triviaGame = gameObject.AddComponent<Trivia>();
		playerEvents = new List<Event> {gestureGame, scramblerGame, triviaGame};
	}
}