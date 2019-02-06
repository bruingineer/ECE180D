using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEvents : Moving_Object {

	Event gestureGame, speechRecognizerGame;
	private List<Event> playerEvents;

	void Awake()
	{
		gestureGame = gameObject.AddComponent<GestureGame>();
		speechRecognizerGame = gameObject.AddComponent<SpeechRecognitionEngine>();
		playerEvents = new List<Event> {gestureGame, speechRecognizerGame};
	}

	void Start()
	{
		StartCoroutine(HandleEvents());
	}

	public void StartEvents()
	{
		StartCoroutine(HandleEvents());
	}

	private IEnumerator HandleEvents()
	{	
		// change to game playing
		// while(true)
		// {	
		// 	yield return playerEvents[Random.Range(0, playerEvents.Count)].StartEvent();
		// }
		yield return true;
	}
}