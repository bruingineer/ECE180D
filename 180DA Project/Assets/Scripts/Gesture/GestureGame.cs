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
	// Change as well for difficulty
	private float repeatRate = 1f;
	private float timerDuration;
	
	// MQTT topics
	public const string topicGestureSent = "gesture";
	private string topicCorrectGesture = "gesture_correct";
	private string stopMessage = "stop";
	

	void Start () {

 		if (SelectedPlayer.current_difficulty == "easy") timerDuration = 11f;
        else if (SelectedPlayer.current_difficulty == "medium") timerDuration = 8f;
        else if (SelectedPlayer.current_difficulty == "hard") timerDuration = 5f; 

		correctGestureFunc = HandleCorrectGesture;
		gestureClient = new GestureClient(topicCorrectGesture, correctGestureFunc);
		m_player = GameObject.Find("Player").GetComponent<Player>();

	timeLeft = GameObject.FindWithTag("timer").GetComponent<Text>();
		gestureText = GameObject.Find(mainText).GetComponent<Text>();
		string chosenGesture = gestures[UnityEngine.Random.Range(0, gestures.Count)].ToUpper();
		curGesture = chosenGesture.ToUpper();
		gestureText.text = curGesture;
		gestureClient.SendMessage(topicGestureSent, chosenGesture);
		
		StartCoroutine("Timer");
		InvokeRepeating("MakeTextBlink", 0, repeatRate);
	}

	IEnumerator MakeTextBlink()
	{
		gestureText.text = "";
		yield return new WaitForSeconds(0.5f);
		gestureText.text = curGesture;
		yield return new WaitForSeconds(0.5f);
	}

	public IEnumerator Timer() 
	{	
		
        while (timerDuration >= 0)
        {   
            timerDuration -= Time.deltaTime;
            int integer = (int)timerDuration;
            if (integer >= 1)
                timeLeft.text = integer.ToString();
            else
                timeLeft.text = "Time's Up";
            yield return null;
        }
			gestureClient.SendMessage(topicGestureSent, stopMessage);
            // change based on if mini game or not
			timeLeft.text = "";
			gestureText.text = "";
            Destroy(gameObject);
	}

	private void HandleCorrectGesture()
	{
		timeLeft.text = "Correct!";
		gestureClient.SendMessage(topicGestureSent, stopMessage);
        SelectedPlayer.current_gesture_pass++;
		StopCoroutine("Timer");
		m_player.MovePlayer();
		gestureText.text = "";
		Destroy(gameObject);
	}
}
