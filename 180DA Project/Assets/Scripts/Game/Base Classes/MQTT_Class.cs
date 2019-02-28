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

public class PlayerMQTT_X : MQTT_Class {
	public int cur_lane_num, numLanes;
	public PlayerMQTT_X(string topic, int numL) : base(topic)
	{
		cur_lane_num = numL / 2;
		numLanes = numL;
	}
	
	protected override void client_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e) 
	{ 
		int lane_num;
		if (Int32.TryParse(System.Text.Encoding.UTF8.GetString(e.Message), out lane_num)) {
			lane_num -= 1;
			if (lane_num != cur_lane_num) {
				cur_lane_num = lane_num;
			}
		} 
	} 

}

public class PlayerMQTT_Y : MQTT_Class {
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
    
	public PlayerMQTT_Y(string topic) : base(topic){
		playerMoved = false;
	}
	protected override void client_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e) 
	{ 
		PlayerMoved = true;
	}
}

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
