using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerEvents : Moving_Object {
	protected Event gestureGame, speechRecognizerGame, triviaGame;

	protected List<Event> playerEvents;

	public abstract void Awake();

	public void StartEvents()
	{
		StartCoroutine(HandleEvents());
	}

	private IEnumerator HandleEvents()
	{	
		while(GameState_Base.gamePlaying)
		{	
			yield return playerEvents[Random.Range(0, playerEvents.Count)].StartEvent();
			Debug.Log("returning from an event");
		}
		yield return true;
	}
}

public abstract class PlayerEvents_Minigame: PlayerEvents {
	public override abstract void Awake();
}

public class PlayerEvents_Gesture_Minigame : PlayerEvents_Minigame {
	public override void Awake()
	{
		gestureGame = gameObject.AddComponent<GestureMiniGame>();
		playerEvents = new List<Event> {gestureGame};
	}
}

public class PlayerEvents_Speech_Minigame : PlayerEvents_Minigame {
	public override void Awake()
	{
		speechRecognizerGame = gameObject.AddComponent<SpeechRecognitionEngine_Minigame>();
		playerEvents = new List<Event> {speechRecognizerGame};
	}
}