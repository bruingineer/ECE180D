using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEvents_MainGame : PlayerEvents {
	public override void Awake()
	{
		gestureGame = gameObject.AddComponent<GestureGame>();
		speechRecognizerGame = gameObject.AddComponent<Speech>();
		playerEvents = new List<Event> {gestureGame, speechRecognizerGame, triviaGame};
		//playerEvents = new List<Event> {gestureGame,  speechRecognizerGame, triviaGame};
		//playerEvents = new List<Event> {speechRecognizerGame, triviaGame};
		//playerEvents = new List<Event> {speechRecognizerGame};
		// playerEvents = new List<Event> {speechgame};
	}
}