using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEvents_Multiplayer : PlayerEvents {
	private string subscribeTopic; 
	public static bool eventReady;
	public static int eventIndex;
	public static string phrase;
	protected Event gestureGame, scramblerGame, triviaGame;

	protected override void Awake() 
	{
	
		subscribeTopic = Multiplayer_Controller.playerHeader + "event";
		publishTopic = Multiplayer_Controller.playerHeader + "request_event";
		multiplayerClient = new EventMultiplayerClient(subscribeTopic);
		eventReady = false;
		gestureGame = gameObject.AddComponent<GestureGame>();
		scramblerGame = gameObject.AddComponent<WordScramble>();
		triviaGame = gameObject.AddComponent<Trivia>();
		playerEvents = new List<Event> {gestureGame, scramblerGame, triviaGame};
	}

	void Update () {
		HandleMultiplayer();
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
		messageOut = false;
	}	
}