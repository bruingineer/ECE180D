using UnityEngine;
using System.Collections;
using System.Net;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
using uPLibrary.Networking.M2Mqtt.Utility;
using uPLibrary.Networking.M2Mqtt.Exceptions;
using System;

public class PlayerMQTT_X : MQTT_Class {
	private bool playerMoved;
	public bool PlayerMoved 
	{
		get
		{
			return playerMoved;
		}
		set
		{
			playerMoved = value;
		}
	}
    
	public PlayerMQTT_X(string topic) : base(topic){
		playerMoved = false;
	}
	protected override void client_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e) 
	{ 
		PlayerMoved = true;
	}
}
