using UnityEngine;
using System.Collections;
using System.Net;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
using uPLibrary.Networking.M2Mqtt.Utility;
using uPLibrary.Networking.M2Mqtt.Exceptions;
using System;

/*
	This class is responsible for abstracting the
	creation of an MQTT client.
 */
public abstract class MQTT_Class {

    protected int portNum = 1883;
	protected string ip = "127.0.0.1";
	protected MqttClient client;

	public MQTT_Class(string topic) 
	{
		CreateClient(topic);
	}
	
	private void CreateClient(string topic) 
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

	// allows client to send a message at the specified topic
	public void SendMessage(string topic, string message)
	{
		client.Publish(topic, System.Text.Encoding.UTF8.GetBytes(message), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, false);
	}

	protected abstract void client_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e);
}
