using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Speech_MiniGame : MonoBehaviour {

	// Use this for initialization
	GameObject canvas;
	int amountCorrectInARow;
	public GameObject SpeechGame;
	public static int curCorrect;
	public static bool eventOn;
	GameObject retry;
	GameObject gameMenu;
	void Start () {
		canvas = GameObject.FindGameObjectWithTag("Canvas");
		eventOn = false;
		amountCorrectInARow = 5;
		curCorrect = 0;
		retry = GameObject.FindGameObjectWithTag("Retry");
		gameMenu = GameObject.FindGameObjectWithTag("Menu");
	}
	
	// Update is called once per frame
	void Update () {
		if (!eventOn && (curCorrect < amountCorrectInARow) && GameState.gamePlaying)
		{
			eventOn = true;
			Instantiate(SpeechGame);
		}
		if (curCorrect >= amountCorrectInARow)
		{
			retry.SetActive(true);
			gameMenu.SetActive(true);
		}
	}
}
