using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEvents : MonoBehaviour {

	public GameObject speechScrambler;
	public GameObject gestureGame;
	private List<GameObject> playerEvents;
	public static bool eventOn;

	void Awake()
	{
		eventOn = false;
		playerEvents = new List<GameObject> {speechScrambler, gestureGame};
	}
	
	void Update () {
		if(!eventOn && !Player.isDead && GameState.gamePlaying) {
			eventOn = true;
			Instantiate(playerEvents[Random.Range(0, playerEvents.Count)]);
		}
	}
}
