using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEvents : MonoBehaviour {

	public GameObject speechScrambler;
	private List<GameObject> playerEvents;
	public static bool eventOn;

	void Awake()
	{
		eventOn = false;
		playerEvents = new List<GameObject> {speechScrambler};
	}
	
	void Update () {
		if(!eventOn && !Player.isDead && GameState.gameStarted) {
			eventOn = true;
			Instantiate(playerEvents[Random.Range(0, playerEvents.Count)]);
		}
	}
}
