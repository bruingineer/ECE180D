using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerEvents : Challenge {

	private float delay = 2f;

	protected List<Event> playerEvents;

	public override void StartChallenge()
	{
		StartCoroutine(HandleEvents());
	}

	protected virtual IEnumerator HandleEvents()
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
	protected override abstract void Awake();
}

public class PlayerEvents_Gesture_Minigame : PlayerEvents_Minigame {
	Event gestureMiniGame;
	protected override void Awake()
	{
		gestureMiniGame = gameObject.AddComponent<GestureMiniGame>();
		playerEvents = new List<Event> {gestureMiniGame};
	}
}

public class PlayerEvents_Scrambler_Minigame : PlayerEvents_Minigame {
	Event scramblerMiniGame;
	protected override void Awake()
	{
		scramblerMiniGame = gameObject.AddComponent<WordScrambleMiniGame>();
		playerEvents = new List<Event> {scramblerMiniGame};
	}
}

public class PlayerEvents_Trivia_Minigame : PlayerEvents_Minigame {
	Event triviaMiniGame;
	protected override void Awake()
	{
		triviaMiniGame = gameObject.AddComponent<TriviaMiniGame>();
		playerEvents = new List<Event> {triviaMiniGame};
	}
}
