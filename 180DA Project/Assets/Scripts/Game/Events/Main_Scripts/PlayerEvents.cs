using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerEvents : Challenge {

	private float delay = 2f;
	protected List<Event> playerEvents;
	protected Event gestureGame, scramblerGame, triviaGame;

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
	protected override void Awake()
	{
		gestureGame = gameObject.AddComponent<GestureMiniGame>();
		playerEvents = new List<Event> {gestureGame};
	}
}

public class PlayerEvents_Scrambler_Minigame : PlayerEvents_Minigame {

	protected override void Awake()
	{
		scramblerGame = gameObject.AddComponent<WordScrambleMiniGame>();
		playerEvents = new List<Event> {scramblerGame};
	}
}

public class PlayerEvents_Trivia_Minigame : PlayerEvents_Minigame {
	protected override void Awake()
	{
		triviaGame = gameObject.AddComponent<TriviaMiniGame>();
		playerEvents = new List<Event> {triviaGame};
	}
}
