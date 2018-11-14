using UnityEngine;
using System.Collections;
using System.Net;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
using uPLibrary.Networking.M2Mqtt.Utility;
using uPLibrary.Networking.M2Mqtt.Exceptions;

using System;

public class PlayerMQTT : MonoBehaviour {

    private const string str_IP = "127.0.0.1";
    private const int int_Port = 1883;
    private const string topic = "localization";
	public static Globals.lane lane_state = Globals.lane.Middle;

    private MqttClient client;
	// Use this for initialization
	void Start () {
		// create client instance 
		client = new MqttClient(IPAddress.Parse(str_IP), int_Port , false , null ); 
		
		// register to message received 
		client.MqttMsgPublishReceived += client_MqttMsgPublishReceived; 
		
		string clientId = Guid.NewGuid().ToString(); 
		client.Connect(clientId); 
		
		// subscribe to the topic "/home/temperature" with QoS 2 
		client.Subscribe(new string[] { topic }, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE }); 

	}
	void client_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e) 
	{ 
		string lane_str = System.Text.Encoding.UTF8.GetString(e.Message);
		Globals.lane lane_enum;
		switch (lane_str) 
		{
			case "Top":
				lane_enum = Globals.lane.Top;
				break;
			case "Middle":
				lane_enum = Globals.lane.Middle;
				break;
			case "Bottom":
				lane_enum = Globals.lane.Bottom;
				break;
			default:
				return;

		}
		Debug.Log("New Enum:" + lane_enum);

		if (lane_enum != lane_state) {
			Debug.Log("change!");
			lane_state = lane_enum;
		}
		else {
			Debug.Log("No change!");
		}
	} 

}
