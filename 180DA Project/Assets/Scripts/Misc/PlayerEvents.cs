using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerEvents : Moving_Object {
	protected Event gestureGame, speechRecognizerGame;

	protected List<Event> playerEvents;

	public abstract void Awake();
	

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
		while(true)
		{	
			yield return playerEvents[Random.Range(0, playerEvents.Count)].StartEvent();
		}
		yield return true;
	}
}

public class PlayerEvents_MainGame : PlayerEvents {
	public override void Awake()
	{
		gestureGame = gameObject.AddComponent<GestureGame>();
		speechRecognizerGame = gameObject.AddComponent<SpeechRecognitionEngine>();
		playerEvents = new List<Event> {gestureGame, speechRecognizerGame};
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
		speechRecognizerGame = gameObject.AddComponent<SpeechRecognitionEngine>();
		playerEvents = new List<Event> {speechRecognizerGame};
	}
}