using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
using uPLibrary.Networking.M2Mqtt.Utility;
using uPLibrary.Networking.M2Mqtt.Exceptions;
using System.Net;

public class GestureClient : MonoBehaviour {
	private List<string> gestrures;
    private string topicGestureCorrect = "gesture_correct";
	public const string topicGestureSent = "gesture";
    public static MqttClient gestureClient;
	public bool messageReceived;
	// Use this for initialization
	void Awake () {
		// create client instance 
		gestureClient = new MqttClient(IPAddress.Parse(GameState.str_IP), GameState.int_Port , false , null ); 
		
		// register to message received 
		gestureClient.MqttMsgPublishReceived += client_MqttMsgPublishReceived; 
		
		string clientId = System.Guid.NewGuid().ToString(); 
		gestureClient.Connect(clientId); 
		
		// subscribe to the topic "/home/temperature" with QoS 2 
		gestureClient.Subscribe(new string[] { topicGestureCorrect }, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE }); 

		messageReceived = false;
	}

	void client_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e) 
	{ 
		GestureGame.correctGestureReceived = true;
	} 
}
