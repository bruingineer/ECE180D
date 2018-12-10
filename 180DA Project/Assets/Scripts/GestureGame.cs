using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
using uPLibrary.Networking.M2Mqtt.Utility;
using uPLibrary.Networking.M2Mqtt.Exceptions;
using System.Net;

using UnityEngine.UI;

public class GestureGame : MonoBehaviour {
	private List<string> gestures;
	private bool handlingCorrectGesture;
	public static bool correctGestureReceived;

	public static int numSucess;
	public static int numFails;
	private Text timeLeft;

	// Use this for initialization
	void Awake () {
		handlingCorrectGesture = false;
		correctGestureReceived = false;
		gestures = new List<string>(){"tpose"};
		GestureClient.gestureClient.Publish(GestureClient.topicGestureSent, System.Text.Encoding.UTF8.GetBytes(gestures[Random.Range(0, gestures.Count)]), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, false);
		StartCoroutine("Timer");
	}

	void Update()
	{
		if (correctGestureReceived && !handlingCorrectGesture)
		{
			numSucess++;
			StopCoroutine("Timer");
			StartCoroutine(HandleCorrectGesture());
		}
	}

	public IEnumerator Timer() 
	{	
			timeLeft = GameObject.FindWithTag("timer").GetComponent<Text>();
            float duration = 8f;
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
			PlayerEvents.eventOn = false;
			timeLeft.text = "";
            Destroy(gameObject);
	}

	private IEnumerator HandleCorrectGesture()
	{
		PlayerMQTT_X.playerMoved = true;
		yield return new WaitForSeconds(Obstacles.obstacleWaitTime);
		PlayerEvents.eventOn = false;
		correctGestureReceived = false;
		Destroy(gameObject);
	}
}
