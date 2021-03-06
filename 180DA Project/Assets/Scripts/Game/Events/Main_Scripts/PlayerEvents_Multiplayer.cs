﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEvents_Multiplayer : PlayerEvents {
	private string subscribeTopic; 
	public static bool eventReady;
	public static int eventIndex;
	public static string phrase;

	protected override void Awake() 
	{
		eventReady = false;
		gestureGame = gameObject.AddComponent<GestureMultiplayerGame>();
		scramblerGame = gameObject.AddComponent<WordScramble>();
		triviaGame = gameObject.AddComponent<Trivia>();
		playerEvents = new List<Event> {gestureGame, scramblerGame, triviaGame};
	}

	void Update () {
		HandleMultiplayer();
	}

	public override void SetUpClient(string header)
	{
		subscribeTopic = header + "event";
		publishTopic = header + "request_event";
		multiplayerClient = new EventMultiplayerClient(subscribeTopic);
	}

	protected override void HandleChallengeReady()
	{
		if (eventReady)
			{
				Debug.Log("Starting Obstacle...");
				eventReady = false;
				StartChallenge();
			}
	}

	protected override IEnumerator HandleEvents()
	{	
		yield return playerEvents[eventIndex].StartEvent(phrase);
		yield return new WaitForSeconds(delay);
		messageOut = false;
	}	
}