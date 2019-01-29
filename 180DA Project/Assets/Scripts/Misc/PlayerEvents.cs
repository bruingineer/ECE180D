using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEvents : MonoBehaviour {

	public GameObject speechScrambler;
	public GameObject gestureGame;
	private List<GameObject> playerEvents;
	public static bool eventOn;
	private GameObject canvas;

	void Awake()
	{
		eventOn = false;
        playerEvents = new List<GameObject> { gestureGame, speechScrambler};
		canvas = GameObject.FindGameObjectWithTag("Canvas");
	}
	
	void Update () {
		if(!eventOn && !Player.isDead && GameState.gamePlaying) {
			eventOn = true;
			Instantiate(playerEvents[Random.Range(0, playerEvents.Count)]);
		}
	}
}
