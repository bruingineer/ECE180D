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
		StartCoroutine(HandleObstacles());
	}

	public void StartObstacles()
	{
		StartCoroutine(HandleObstacles());
	}

	private IEnumerator HandleObstacles()
	{	
		// change to game playing
		while(true)
		{	
			yield return obstacles[Random.Range(0, obstacles.Count)].StartObstacle();
		}
	}
}