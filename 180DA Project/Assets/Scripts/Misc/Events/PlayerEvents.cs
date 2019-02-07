using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerEvents : Moving_Object {
	protected Event gestureGame, speechRecognizerGame, triviaGame;

	protected List<Event> playerEvents;

	public abstract void Awake();
	

	void Start()
	{
		//Debug.Log("PlayerEvents.Start()");
		StartCoroutine(HandleEvents());
	}

	public void StartEvents()
	{
		StartCoroutine(HandleEvents());
	}

	private IEnumerator HandleEvents()
	{	
		//Debug.Log("PlayerEvents.StartEvents");
		// change to game playing
		while(true)
		{	
			yield return playerEvents[Random.Range(0, playerEvents.Count)].StartEvent();
			Debug.Log("returning from an event");
		}
		yield return true;
	}
}
	



public abstract class PlayerEvents_Minigame: PlayerEvents {
	public override abstract void Awake();
	private int numCorrect = 5;
	public int curCorrect = 0;
}

public class PlayerEvents_Gesture_Minigame : PlayerEvents_Minigame {
	public override void Awake()
	{
		gestureGame = gameObject.AddComponent<GestureGame>();
		playerEvents = new List<Event> {gestureGame};
	}
}

public class PlayerEvents_Speech_Minigame : PlayerEvents_Minigame {
	public override void Awake()
	{
		speechRecognizerGame = gameObject.AddComponent<SpeechRecognitionEngine2>();
		playerEvents = new List<Event> {speechRecognizerGame};
	}
}