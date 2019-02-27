using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerEvents : Moving_Object {

	private float delay = 2f;

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
			yield return new WaitForSeconds(delay);
		}
		yield return true;
	}
}

public abstract class PlayerEvents_Minigame: PlayerEvents {
	public override abstract void Awake();
}

public class PlayerEvents_Gesture_Minigame : PlayerEvents_Minigame {
	Event gestureMiniGame;
	public override void Awake()
	{
		gestureMiniGame = gameObject.AddComponent<GestureMiniGame>();
		playerEvents = new List<Event> {gestureMiniGame};
	}
}

public class PlayerEvents_Scrambler_Minigame : PlayerEvents_Minigame {
	Event scramblerMiniGame;
	public override void Awake()
	{
		scramblerMiniGame = gameObject.AddComponent<WordScrambleMiniGame>();
		playerEvents = new List<Event> {scramblerMiniGame};
	}
}

public class PlayerEvents_Trivia_Minigame : PlayerEvents_Minigame {
	Event triviaMiniGame;
	public override void Awake()
	{
		triviaMiniGame = gameObject.AddComponent<TriviaMiniGame>();
		playerEvents = new List<Event> {triviaMiniGame};
	}
}
