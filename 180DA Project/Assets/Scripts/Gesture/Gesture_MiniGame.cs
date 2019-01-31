using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gesture_MiniGame : MonoBehaviour {

	// TODO fix this
	int amountCorrectInARow;
	public GameObject GestureGame;
	public static int curCorrect;
	public static bool eventOn;
	public GameObject canvas;
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
			Instantiate(GestureGame);
		}
		if (curCorrect >= amountCorrectInARow)
		{
			retry.SetActive(true);
			gameMenu.SetActive(true);
		}
	}
}
