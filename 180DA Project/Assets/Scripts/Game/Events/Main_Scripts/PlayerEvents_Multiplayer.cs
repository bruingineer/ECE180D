using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEvents_Multiplayer : PlayerEvents {
	private string subscribeTopic; 
	ObstacleMultiplayerClient playerEventMultiplayerClient;
	private string publishTopic;
	private bool messageOut;
	public static bool eventReady;
	public static int eventIndex;
	public static string phrase;
	protected Event gestureGame, scramblerGame, triviaGame;

	protected override void Awake() 
	{
		subscribeTopic = Multiplayer_Controller.playerHeader + "event";
		publishTopic = Multiplayer_Controller.playerHeader + "request_event";
		playerEventMultiplayerClient = new ObstacleMultiplayerClient(subscribeTopic);
		messageOut = false;
		eventReady = false;
		gestureGame = gameObject.AddComponent<GestureGame>();
		scramblerGame = gameObject.AddComponent<WordScramble>();
		triviaGame = gameObject.AddComponent<Trivia>();
		playerEvents = new List<Event> {gestureGame, scramblerGame, triviaGame};
	}

	void Update () {
		if (GameState_Base.gamePlaying)
		{
			if (!messageOut)
			{
				messageOut = true;
				playerEventMultiplayerClient.SendMessage(publishTopic, "requested");
				Debug.Log("Sending message to server!");
			}
			if (eventReady)
			{
				Debug.Log("Starting Event...");
				eventReady = false;
				StartEvents();
			}
		}
	}

	protected override IEnumerator HandleEvents()
	{	
		yield return playerEvents[eventIndex].StartEvent(phrase);
		messageOut = false;
	}	
}