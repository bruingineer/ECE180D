using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
using uPLibrary.Networking.M2Mqtt.Utility;
using uPLibrary.Networking.M2Mqtt.Exceptions;
using System.Net;
using System;

public class GestureClient : MQTT_Class {
	
	private Action correctGestureFunc;
	public GestureClient(string topic) : base(topic) {}
	

	protected override void client_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e) 
	{
        Debug.Log("correct msg received");
		GestureGame.gestureCorrect = true;
	} 
}
