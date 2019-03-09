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

/*
	This class is used to handle the Gesture Game,
	in which a player must match a gesture given to them.
	The gesture is analyzed using OpenPose.
*/
public abstract class GestureGame : Event {
	// Objects

	/*
		These will be used to send through the
		GestureClient so OpenPose knows what
		gesture to look out for.

		Use this for adding new gestures.
	*/

	// Gesture names
	public static string tpose = "tpose";
	public static string fieldGoal = "fieldgoal";
	public static string rightHandDab = "dab";
	public static string rightHandWave = "righthandwave";
	public static string leftHandRaise = "lefthandraise";
	public static string rightHandRaise = "righthandraise";
	public static string leftHandWave = "lefthandwave";
	private List<string> gestures = new List<string>()
			{
				tpose, 
				fieldGoal,
				rightHandWave,
				leftHandWave,
				rightHandRaise,
				leftHandRaise,
				rightHandDab
			};

	private Dictionary<string, string> gestureStringBeautify = new Dictionary<string, string> {
		{tpose, "TPose"},
		{fieldGoal, "Touchdown"},
		{rightHandWave, "Wave Right Hand"},
		{leftHandWave, " Wave Left Hand"},
		{rightHandRaise, "Raise Right Hand"},
		{leftHandRaise, "Raise Left Hand"},
		{rightHandDab, "Dab Right Side"}
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
	protected const string gestureTopicString = "gesture";
	protected const string gestureCorrectTopicString = "gesture_correct";
	protected string topicGestureSent;

	// this topic is used to know if OpenPose correctly
	// identified the gesture (subscribe)
	protected string topicCorrectGesture;

	protected override void Update()
	{
		base.Update();
		if (gestureCorrect && !handledCorrect)
			HandleCorrectEvent();
	}

	// This function is overridden from the Event base class,
	// and is used to add to the Awake function of the Event class
	// for initialization
	protected override void Awake()
    {
			SetUpTopics();
			gestureClient = new GestureClient(topicCorrectGesture);
			gestureText = GameObject.FindWithTag("word").GetComponent<TextMeshProUGUI>();
			Msg = GameObject.FindWithTag("msg").GetComponent<TextMeshProUGUI>();
			base.Awake();
    }

	protected virtual void SetUpTopics()
	{
		topicGestureSent = gestureTopicString;
		topicCorrectGesture = gestureCorrectTopicString;
	}

	// This function is overriden from the Event base class,
	// and is used to set up the event each time it is chosen
	protected override void SetUpEvent(string phrase = null)
	{
		curGesture = phrase != null ? phrase : gestures[UnityEngine.Random.Range(0, gestures.Count)];
		gestureText.text = gestureStringBeautify[curGesture];
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
	protected override void HandleIncorrectEvent()
	{
		gestureClient.SendMessage(topicGestureSent, stopMessage);
		Reset();
	}

	// function called when event is correct
	protected override void HandleCorrectEvent()
	{
		Debug.Log("Correct");
		// set to true as to not repeat the function
		gestureClient.SendMessage(topicGestureSent, stopMessage);
		handledCorrect = true;
		timerStopped = true;
		Msg.text = "Correct!";
		// increment SelectedPlayer's gesture pass counter
        // and add the time left on the timer to the timer avg counter
		HandleCorrectAction();
		StartCoroutine(ResetCorrect());
	}

	protected override IEnumerator ResetCorrect()
	{
		yield return new WaitForSeconds(endDisplayTime);
		Reset();
		eventCorrect = true;
		gestureCorrect = false;
        handledCorrect = false;
	}

	/*
		This function is used to reset some UI elements, stop OpenPose
		from continuing to recognize gesture, and delay by specified amount
		of time
	*/
	protected override void Reset()
	{
		Msg.text = "";
		gestureText.text = "";
	}

}

public class GestureMiniGame : GestureGame {

	// number of current gestures correct increments when it is a minigame
	protected override void HandleCorrectAction()
	{
		GameState_Event_Minigame.curCorrect++;
	}

	// number of correct gestures resets to 0 when incorrect
	protected override void HandleIncorrectEvent()
	{
		GameState_Event_Minigame.curCorrect = 0;
		base.HandleIncorrectEvent();
	}
}

public class GestureMultiplayerGame : GestureGame
{
	// Change for OpenPose here!
	private string MQTTHeader = "A";
	protected override void SetUpTopics()
	{
		topicGestureSent = MQTTHeader + '/' + gestureTopicString;
		topicCorrectGesture = MQTTHeader + '/' + gestureCorrectTopicString;
	}
}

public class GestureMainGame : GestureGame
{
	protected override void HandleCorrectAction()
	{
		SelectedPlayer.current_gesture_pass++;
        SelectedPlayer.current_g_timer_avg += Event.curTime;
		base.HandleCorrectAction();
	}

	protected override void HandleIncorrectEvent()
	{
		SelectedPlayer.current_gesture_fail++;
		base.HandleIncorrectEvent();
	}
}
