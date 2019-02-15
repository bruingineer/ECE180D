using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
using uPLibrary.Networking.M2Mqtt.Utility;
using uPLibrary.Networking.M2Mqtt.Exceptions;
using System.Net;
using System;

/*
	The GestureClient class sets up an MQTT client that will be used
	for during the Gesture Game. 
*/
public class GestureClient : MQTT_Class {
	
	// Constructor that will initialize the MQTT_Class
	public GestureClient(string topic) : base(topic) {}

	/*
		This function gets called whenever this GestureClient class
		receives a message by being described to the specified topic.
		When it gets called, it accesses the gestureCorrect static variable
		in the GestureGame class and sets it to true, indicating the gesture
		was correct.
	*/
	protected override void client_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e) 
	{
		GestureGame.gestureCorrect = true;
	} 
}
