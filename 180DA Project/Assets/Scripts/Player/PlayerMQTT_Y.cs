using UnityEngine;
using System.Collections;
using System.Net;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
using uPLibrary.Networking.M2Mqtt.Utility;
using uPLibrary.Networking.M2Mqtt.Exceptions;

using System;

public class PlayerMQTT_Y : MQTT_Class {
	public int cur_lane_num;
	public PlayerMQTT_Y(string topic, int start_lane) : base(topic)
	{
		cur_lane_num = start_lane;
	}
	
	protected override void client_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e) 
	{ 
		int lane_num;
		if (Int32.TryParse(System.Text.Encoding.UTF8.GetString(e.Message), out lane_num)) {
			lane_num = GameState.numLanes - lane_num + 1;
			if (lane_num != cur_lane_num) {
				cur_lane_num = lane_num;
			}
		} 
	} 

}
