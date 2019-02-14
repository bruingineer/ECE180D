using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
using uPLibrary.Networking.M2Mqtt.Utility;
using uPLibrary.Networking.M2Mqtt.Exceptions;
using System.Net;
using System;
using UnityEngine.UI;
using TMPro;

public class GestureGame : Event {

	// Objects
	// available gestures
	private List<string> gestures = new List<string>()
	{
		"tpose", 
		"fieldgoal"
	};

	private TextMeshProUGUI gestureText;
	//private Text gestureText;
	private TextMeshProUGUI Msg;
	string mainText = "Command_Text"; 
	private GestureClient gestureClient;
	private string curGesture;
	public static bool gestureCorrect = false;

	private bool handledCorrect = false;
	
	
	// MQTT topics
	public const string topicGestureSent = "gesture";
	private string topicCorrectGesture = "gesture_correct";
	private string stopMessage = "stop";

	protected override void Initialize()
    {
			gestureClient = new GestureClient(topicCorrectGesture);
			gestureText = GameObject.FindWithTag("word").GetComponent<TextMeshProUGUI>();
			Msg = GameObject.FindWithTag("msg").GetComponent<TextMeshProUGUI>();
			SetUp();
    }
	protected override void SetUpEvent()
	{
		string chosenGesture = gestures[UnityEngine.Random.Range(0, gestures.Count)].ToUpper();
		curGesture = chosenGesture.ToUpper();
		gestureText.text = curGesture;
		Msg.text = "Do This:";
		gestureClient.SendMessage(topicGestureSent, chosenGesture);
	}

	void Update()
	{
		if (gestureCorrect && !handledCorrect)
			HandleCorrectGesture();
	}

	protected override IEnumerator MakeTextBlink()
	{

		while (true && !timerPaused){
			gestureText.text = "";
			yield return new WaitForSeconds(repeatRate);
			gestureText.text = curGesture;
			yield return new WaitForSeconds(repeatRate);
		}
		gestureText.text = "";
	}

	protected override IEnumerator HandleIncorrect()
	{
		timerPaused = true;
		gestureClient.SendMessage(topicGestureSent, stopMessage);
		gestureText.text = "";
		Msg.text = "";
		SelectedPlayer.current_gesture_fail++;
		HandleIncorrectMiniGame();
		yield return StartCoroutine("Delay");
	}

	public void HandleCorrectGesture()
	{
        Debug.Log("Handling Correct Gesture");
		handledCorrect = true;
		timerPaused = true;
        m_player.MovePlayer();
		Msg.text = "Correct!";
		gestureClient.SendMessage(topicGestureSent, stopMessage);
        SelectedPlayer.current_gesture_pass++;
		HandleCorrectMiniGame();
		StartCoroutine("HandleCorrectCoroutine");
	}

	private IEnumerator HandleCorrectCoroutine()
	{
			Msg.text = "";
			gestureText.text = "";
			yield return StartCoroutine("Delay");
			eventCorrect = true;
            gestureCorrect = false;
            handledCorrect = false;
			
    }

	protected virtual void HandleCorrectMiniGame() {}
	protected virtual void HandleIncorrectMiniGame() {}
}

// potentially change
public class GestureMiniGame : GestureGame {
	protected override void HandleCorrectMiniGame()
	{
		GameState_Event_Minigame.curCorrect++;
	}

	protected override void HandleIncorrectMiniGame()
	{
		GameState_Event_Minigame.curCorrect = 0;
	}

	protected override void SetUp() {}
}
