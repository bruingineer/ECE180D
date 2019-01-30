using UnityEngine;
using System.Collections;
using System.Net;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
using uPLibrary.Networking.M2Mqtt.Utility;
using uPLibrary.Networking.M2Mqtt.Exceptions;
using System;

public class MQTT_Class {
	protected string topic;
    protected int portNum = 1883;
	protected string ip = "127.0.0.1";
	protected MqttClient client;
	
	protected void CreateClient(string topic) 
	{
		// create client instance 
		client = new MqttClient(IPAddress.Parse(ip), portNum, false , null ); 
		
		// register to message received 
		client.MqttMsgPublishReceived += client_MqttMsgPublishReceived; 
		
		string clientId = Guid.NewGuid().ToString(); 
		client.Connect(clientId); 
		
		// subscribe to the topic "/home/temperature" with QoS 2 
		client.Subscribe(new string[] { topic }, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE }); 
	}

	public MqttClient GetClient()
	{
		return client;
	}

	protected virtual void client_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e) {}
}
