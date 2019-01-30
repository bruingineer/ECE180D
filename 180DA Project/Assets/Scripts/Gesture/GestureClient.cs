using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
using uPLibrary.Networking.M2Mqtt.Utility;
using uPLibrary.Networking.M2Mqtt.Exceptions;
using System.Net;

public class GestureClient : MQTT_Class {
	
	public GestureClient()
	{
		topic = "gesture_correct";
		CreateClient(topic);
	}

	protected override void client_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e) 
	{ 
		GestureGame.correctGestureReceived = true;
	} 
}
