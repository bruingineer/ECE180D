using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
using uPLibrary.Networking.M2Mqtt.Utility;
using uPLibrary.Networking.M2Mqtt.Exceptions;
using System.Net;

public class GestureGame : MonoBehaviour {
	private List<string> gestrures;
    private const string topic = "gesture";
    private MqttClient client;
	// Use this for initialization
	void Start () {
		// create client instance 
		client = new MqttClient(IPAddress.Parse(GameState.str_IP), GameState.int_Port , false , null ); 
		
		// register to message received 
		client.MqttMsgPublishReceived += client_MqttMsgPublishReceived; 
		
		string clientId = System.Guid.NewGuid().ToString(); 
		client.Connect(clientId); 
		
		// subscribe to the topic "/home/temperature" with QoS 2 
		client.Subscribe(new string[] { topic }, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE }); 
		gestrures = new List<string>(){"tpose"};
		client.Publish(topic, System.Text.Encoding.UTF8.GetBytes(gestrures[Random.Range(0, gestrures.Count)]), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, true);
	}

	void client_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e) 
	{ 
		Debug.Log(System.Text.Encoding.UTF8.GetString(e.Message));
	} 

	void Update () {
		
	}
}
