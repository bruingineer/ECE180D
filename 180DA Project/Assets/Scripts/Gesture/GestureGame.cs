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

	// Objects
	private List<string> gestures;
	private Text gestureText;
	private Text timeLeft;
	string commandText = "Command_Text"; 
	private Player m_player;
	private GestureClient gestureClient;

	// States
	private bool handlingCorrectGesture;
	public static bool correctGestureReceived;
	private static string curGesture;
	public const string topicGestureSent = "gesture";
	

	void Start () {
		gestureClient = new GestureClient();
		m_player = GameObject.Find("Player").GetComponent<Player>();
		gestureText = GameObject.Find(commandText).GetComponent<Text>();
		handlingCorrectGesture = false;
		correctGestureReceived = false;
		gestures = new List<string>(){"tpose", "fieldgoal"};
		string chosenGesture = gestures[UnityEngine.Random.Range(0, gestures.Count)];
		gestureText.text = chosenGesture.ToUpper();
		
		gestureClient.GetClient().Publish(topicGestureSent, System.Text.Encoding.UTF8.GetBytes(chosenGesture), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, false);
		StartCoroutine("Timer");
		curGesture = gestureText.text;
	}

	IEnumerator makeTextBlink()
	{
		while (true)
		{
			gestureText.text = "";
			yield return new WaitForSeconds(0.5f);
			gestureText.text = curGesture;
			yield return new WaitForSeconds(0.5f);
			if (timeLeft.text == "Time's Up") break;
		}
	}

	void Update()
	{
		StartCoroutine("makeTextBlink");
        if (correctGestureReceived && !handlingCorrectGesture)
        {
			timeLeft.text = "Correct!";
			gestureClient.GetClient().Publish(topicGestureSent, System.Text.Encoding.UTF8.GetBytes("stop"), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, false);
			handlingCorrectGesture = true;
            //numSucess++;
            SelectedPlayer.current_gesture_pass++;
            Debug.Log("current_gesture_pass++");
			StopCoroutine("Timer");
			StartCoroutine(HandleCorrectGesture());
		}
	}

	public IEnumerator Timer() 
	{	
		timeLeft = GameObject.FindWithTag("timer").GetComponent<Text>();
        float duration = 11f;

        //Change time allowed to perform gesture based on difficulty selected
        if (SelectedPlayer.current_difficulty == "easy") duration = 11f;
        else if (SelectedPlayer.current_difficulty == "medium") duration = 8f;
        else if (SelectedPlayer.current_difficulty == "hard") duration = 5f; 

        while (duration >= 0)
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


            SelectedPlayer.current_gesture_fail++;
            Debug.Log("current_gesture_fail++");
            yield return new WaitForSeconds(1);
			Gesture_MiniGame.curCorrect = 0;
			timeLeft.text = "";
			gestureText.text = "";
			PlayerEvents.eventOn = false;
			Gesture_MiniGame.eventOn = false;
            Destroy(gameObject);
	}

	private IEnumerator HandleCorrectGesture()
	{
		
		m_player.MovePlayer();
		yield return new WaitForSeconds(3f);
		PlayerEvents.eventOn = false;
		Gesture_MiniGame.eventOn = false;
		correctGestureReceived = false;
		gestureText.text = "";
		Gesture_MiniGame.curCorrect++;
		Destroy(gameObject);
	}
}
