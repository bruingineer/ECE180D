using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEvents_MainGame : PlayerEvents {
	public override void Awake()
	{
		gestureGame = gameObject.AddComponent<GestureGame>();
		speechRecognizerGame = gameObject.AddComponent<SpeechRecognitionEngine>();
		triviaGame = gameObject.AddComponent<Trivia>();
		playerEvents = new List<Event> {gestureGame}; // speechRecognizerGame, triviaGame};
	}
}