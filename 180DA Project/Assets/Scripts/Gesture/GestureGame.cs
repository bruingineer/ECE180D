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

public class GestureGame : Event {

	// Objects
	// available gestures
	private List<string> gestures = new List<string>()
	{
		"tpose", 
		"fieldgoal"
	};
	private Text gestureText;
	string mainText = "Command_Text"; 
	private GestureClient gestureClient;
	private string curGesture;
	Action correctGestureFunc;
	
	// MQTT topics
	public const string topicGestureSent = "gesture";
	private string topicCorrectGesture = "gesture_correct";
	private string stopMessage = "stop";

	protected override void SetUp()
    {
		correctGestureFunc = HandleCorrectGesture;
		gestureClient = new GestureClient(topicCorrectGesture, correctGestureFunc);
		gestureText = GameObject.Find(mainText).GetComponent<Text>();
    }

	public override IEnumerator StartEvent()
	{
		string chosenGesture = gestures[UnityEngine.Random.Range(0, gestures.Count)].ToUpper();
		curGesture = chosenGesture.ToUpper();
		gestureText.text = curGesture;
		gestureClient.SendMessage(topicGestureSent, chosenGesture);
		yield return StartCoroutine(StartTimer());
	}

	protected override IEnumerator MakeTextBlink()
	{
		gestureText.text = !gestureText ? curGesture : "";
		yield return new WaitForSeconds(repeatRate);
	}

	protected override IEnumerator HandleIncorrect()
	{
		gestureClient.SendMessage(topicGestureSent, stopMessage);
		gestureText.text = "";
		SelectedPlayer.current_gesture_fail++;
		HandleIncorrectMiniGame();
		yield return StartCoroutine(DelayAndEndTimer());
	}

	private void HandleCorrectGesture()
	{
		timerPaused = true;
		m_player.MovePlayer();
		gestureText.text = "Correct!";
		gestureClient.SendMessage(topicGestureSent, stopMessage);
        SelectedPlayer.current_gesture_pass++;
		HandleCorrectMiniGame();
		StartCoroutine(DelayAndEndTimer());
	}

	protected virtual void HandleCorrectMiniGame() {}
	protected virtual void HandleIncorrectMiniGame() {}
}

public class GestureMiniGame : GestureGame {
	protected override void HandleCorrectMiniGame()
	{
		GetComponent<PlayerEvents_Gesture_Minigame>().curCorrect++;
	}

	protected override void HandleIncorrectMiniGame()
	{
		GetComponent<PlayerEvents_Gesture_Minigame>().curCorrect = 0;
	}
}
