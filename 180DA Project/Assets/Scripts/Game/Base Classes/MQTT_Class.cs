using UnityEngine;
using System.Collections;
using System.Text;
using System.Net;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
using uPLibrary.Networking.M2Mqtt.Utility;
using uPLibrary.Networking.M2Mqtt.Exceptions;
using System;
using System.Collections.Generic;

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
		Debug.Log("Sending " + message + " on " + topic);
		client.Publish(topic, System.Text.Encoding.UTF8.GetBytes(message), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, false);
	}

	protected abstract void client_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e);
}

public class MQTTHelper : MQTT_Class
{
    private string _topic;
    //Class to be used for simple MQTT functions, often just requiring a message to publish 
    public MQTTHelper(string topic) : base(topic) {
         _topic = topic;
    }

    public void CheckIfTrainingComplete(string training)
    {
        bool selectedTrainingComplete;
        switch (training)
        {
            case "gesture_training":
                selectedTrainingComplete = SelectedPlayer.gesture_training;
                break;
            case "laser_training":
                selectedTrainingComplete = SelectedPlayer.laser_training;
                break;
            case "unscramble_training":
                selectedTrainingComplete = SelectedPlayer.unscramble_training;
                break;
            case "trivia_training":
                selectedTrainingComplete = SelectedPlayer.unscramble_training;
                break;
            // error
            default:
                return;
        }

        if (selectedTrainingComplete)
            return;

        SendMessage(_topic, string.Format("UPDATE players SET {0}=1 WHERE id = {1}",
                                     training, SelectedPlayer.id));
    }

    protected override void client_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
    { }
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

public class MultiplayerClient : MQTT_Class {
	private bool started = false;
	public MultiplayerClient(string topic) : base(topic) {}

	protected override void client_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e) 
	{
		string message = System.Text.Encoding.UTF8.GetString(e.Message);
		Debug.Log(message);
		if(e.Topic == Multiplayer_Controller.serverTopic)
		{
			if ((message == "player1") || (message == "player2"))
			{
				Multiplayer_Controller.playerHeader = message + '/';
				Multiplayer_Controller.playerConnected = true;
			}
		} else if (e.Topic == Multiplayer_Controller.gameStateTopic && !started)
		{
			Multiplayer_Controller.gameStarted = true;
			started = true;
		} else if (e.Topic == Multiplayer_Controller.winnerTopic)
		{
			if (message == "loser")
				Multiplayer_Controller.lost = true;
			else if (message == "winner")
				Multiplayer_Controller.won = true;
		}
	} 

	public void Subscribe(string[] topics)
	{
		Debug.Log("Subscribing to topics...");
		client.Subscribe(topics, CreateQOSArray(topics.Length));
	}

	private byte[] CreateQOSArray(int size)
	{
		List<byte> qosArray = new List<byte>();
		for(int i = 0; i < size; i++)
			qosArray.Add(MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE);
		return qosArray.ToArray();
	}
}

public class ObstacleMultiplayerClient : MQTT_Class
{
	protected ObstacleInfo obstacleInfo;
	public ObstacleMultiplayerClient(string topic) : base(topic) {}
	protected override void client_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e) 
	{
		string message = System.Text.Encoding.UTF8.GetString(e.Message);
		obstacleInfo = JsonUtility.FromJson<ObstacleInfo>(message);
		int index;
		switch (obstacleInfo.challenge)
		{
			case "small_lasers":
				index = 0;
				break;
			case "big_lasers":
				index = 1;
				break;
			default:
				index = -1;
				break;
		}
		Obstacles_Multiplayer.obstacleIndex = index;
		Obstacles_Multiplayer.laserPositions = obstacleInfo.data;
		Obstacles_Multiplayer.obstacleReady = true;
	} 

	protected class ObstacleInfo
	{
		public string challenge;
		public List<int> data;
	}
}

public class EventMultiplayerClient : MQTT_Class
{
	protected EventInfo eventInfo;
	public EventMultiplayerClient(string topic) : base(topic) {}
	protected override void client_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e) 
	{
		string message = System.Text.Encoding.UTF8.GetString(e.Message);
		eventInfo = JsonUtility.FromJson<EventInfo>(message);
		int index;
		switch (eventInfo.challenge)
		{
			case "gesture":
				index = 0;
				break;
			case "word":
				index = 1;
				break;
			case "trivia":
				index = 2;
				break;
			default:
				index = -1;
				break;
		}
		Debug.Log(eventInfo.challenge);
		Debug.Log(index);
		PlayerEvents_Multiplayer.eventIndex = index;
		PlayerEvents_Multiplayer.phrase = eventInfo.data;
		PlayerEvents_Multiplayer.eventReady = true;
	} 

	protected class EventInfo
	{
		public string challenge;
		public string data;
	}
}

