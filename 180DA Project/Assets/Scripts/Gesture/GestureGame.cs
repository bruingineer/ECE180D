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

	/*
		These will be used to send through the
		GestureClient so OpenPose knows what
		gesture to look out for.

		Use this for adding new gestures.
	*/
	private List<string> gestures = new List<string>()
			{
				"tpose", 
				"fieldgoal"
			};


	// TODO change later so there's only one it accesses
	// should only be one on the canvas it accesses
	// ask Jose if ok to merge
	private TextMeshProUGUI gestureText;

	// TODO Ask Jose if this can be merged as well
	private TextMeshProUGUI Msg;

	// Handle communication with OpenPose
	private GestureClient gestureClient;

	// Variables

	// TODO check if any of these are shared with SpeechGames

	// used to know what gesture was chosen
	private string curGesture;

	// used to know if GestureClient received a correct gesture
	public static bool gestureCorrect = false;

	// MQTT messages
	// This is used to tell OpenPose to stop looking for a gesture
	private string stopMessage = "stop";

	// this is used to only call the HandleCorrect function once
	private bool handledCorrect = false;

	// MQTT topics
	
	// this topic is used to send the gesture to OpenPose (publish)
	public const string topicGestureSent = "gesture";

	// this topic is used to know if OpenPose correctly
	// identified the gesture (subscribe)
	private string topicCorrectGesture = "gesture_correct";

	void Update()
	{
		if (gestureCorrect && !handledCorrect)
			StartCoroutine(HandleCorrectEvent());
	}

	// This function is overridden from the Event base class,
	// and is used to add to the Awake function of the Event class
	// for initialization
	protected override void Event_Initializer()
    {
			gestureClient = new GestureClient(topicCorrectGesture);
			gestureText = GameObject.FindWithTag("word").GetComponent<TextMeshProUGUI>();
			Msg = GameObject.FindWithTag("msg").GetComponent<TextMeshProUGUI>();
    }

	// This function is overriden from the Event base class,
	// and is used to set up the event each time it is chosen
	protected override void SetUpEvent()
	{
		// choose the gesture from the gestures list
		curGesture = gestures[UnityEngine.Random.Range(0, gestures.Count)];
		gestureText.text = curGesture.ToUpper();
		Msg.text = "Do This:";
		// tell OpenPose to start looking for this gesture
		gestureClient.SendMessage(topicGestureSent, curGesture);
	}

	// while the event is still playing, make the text blink,
	// by alternating from an empty string to the current gesture
	protected override IEnumerator MakeTextBlink()
	{
		while (!timerStopped){
			gestureText.text = gestureText.text == "" ? curGesture : "";
			yield return new WaitForSeconds(repeatRate);
		}
		yield return null;
	}

	/*
		This gets called whenever the timer runs out, signifying an
		incorrect event.
	*/
	protected override IEnumerator HandleIncorrectEvent()
	{
		yield return StartCoroutine(Delay());
		// incremement field for SelectedPlayer database
		SelectedPlayer.current_gesture_fail++;
		// reset based on if minigame or not
	}

	// function called when event is correct
	protected override IEnumerator HandleCorrectEvent()
	{
		// set to true as to not repeat the function
		handledCorrect = true;
		// increment SelectedPlayer database
		SelectedPlayer.current_gesture_pass++;
		Msg.text = "Correct!";
		HandleCorrectAction();
		yield return StartCoroutine(Delay());
		// reset variables
		eventCorrect = true;
		gestureCorrect = false;
        handledCorrect = false;
	}

	/*
		This function is used to reset some UI elements, stop OpenPose
		from continuing to recognize gesture, and delay by specified amount
		of time
	*/
	protected override IEnumerator Delay()
	{
		Msg.text = "";
		gestureText.text = "";
		// send a stop message to OpenPose to stop looking at messages
		gestureClient.SendMessage(topicGestureSent, stopMessage);
		yield return base.Delay();
	}

	// The correct action here moves the player (for the main game)
	protected override void HandleCorrectAction() 
	{
		m_player.MovePlayer();
	}
}

// potentially change
public class GestureMiniGame : GestureGame {

	// number of current gestures correct increments when it is a minigame
	protected override void HandleCorrectAction()
	{
		GameState_Event_Minigame.curCorrect++;
	}

	// number of correct gestures resets to 0 when incorrect
	protected override IEnumerator HandleIncorrectEvent()
	{
		GameState_Event_Minigame.curCorrect = 0;
		yield return base.HandleIncorrectEvent();
	}
}
