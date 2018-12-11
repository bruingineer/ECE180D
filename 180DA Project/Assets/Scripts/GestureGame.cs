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

public class GestureGame : MonoBehaviour {
	private List<string> gestures;
	private bool handlingCorrectGesture;
	public static bool correctGestureReceived;
	private Text gestureText;
	public static int numSucess;
	public static int numFails;
	private Text timeLeft;

	// Use this for initialization
	void Awake () {
		gestureText = GameObject.FindGameObjectWithTag("Gesture_Text").GetComponent<Text>();
		handlingCorrectGesture = false;
		correctGestureReceived = false;
		gestures = new List<string>(){"tpose"};
		string chosenGesture = gestures[UnityEngine.Random.Range(0, gestures.Count)];
		gestureText.text = chosenGesture.ToUpper();
		GestureClient.gestureClient.Publish(GestureClient.topicGestureSent, System.Text.Encoding.UTF8.GetBytes(chosenGesture), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, false);
		StartCoroutine("Timer");
	}

	void Update()
	{
		if (correctGestureReceived && !handlingCorrectGesture)
		{
			timeLeft.text = "Correct!";
			handlingCorrectGesture = true;
			numSucess++;
			StopCoroutine("Timer");
			StartCoroutine(HandleCorrectGesture());
		}
	}

	public IEnumerator Timer() 
	{	
			timeLeft = GameObject.FindWithTag("timer").GetComponent<Text>();
            float duration = 11f;
            while(duration >= 0)
            {   
                duration -= Time.deltaTime;
                int integer = (int)duration;
                if (integer >= 1)
                    timeLeft.text = integer.ToString();
                else
                {
                    timeLeft.text = "Time's Up";
                }
                yield return null;
            }
            
            yield return new WaitForSeconds(1);
			Gesture_MiniGame.curCorrect = 0;
			PlayerEvents.eventOn = false;
			Gesture_MiniGame.eventOn = false;
			timeLeft.text = "";
			gestureText.text = "";
            Destroy(gameObject);
	}

	private IEnumerator HandleCorrectGesture()
	{
		
		PlayerMQTT_X.playerMoved = true;
		yield return new WaitForSeconds(3f);
		PlayerEvents.eventOn = false;
		Gesture_MiniGame.eventOn = false;
		correctGestureReceived = false;
		gestureText.text = "";
		Gesture_MiniGame.curCorrect++;
		Destroy(gameObject);
	}
}
