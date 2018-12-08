using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
using uPLibrary.Networking.M2Mqtt.Utility;
using uPLibrary.Networking.M2Mqtt.Exceptions;
using System.Net;

public class GestureGame : MonoBehaviour {
	private List<string> gestures;
	private bool handlingCorrectGesture;
	public static bool correctGestureReceived;
	// Use this for initialization
	void Awake () {
		handlingCorrectGesture = false;
		correctGestureReceived = false;
		gestures = new List<string>(){"tpose"};
		GestureClient.gestureClient.Publish(GestureClient.topicGestureSent, System.Text.Encoding.UTF8.GetBytes(gestures[Random.Range(0, gestures.Count)]), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, false);
	}

	void Update()
	{
		if (correctGestureReceived && !handlingCorrectGesture)
		{
			handlingCorrectGesture = true;
			StartCoroutine(HandleCorrectGesture());
		}
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
