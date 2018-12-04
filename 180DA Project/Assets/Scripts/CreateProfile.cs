using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Net;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
using uPLibrary.Networking.M2Mqtt.Utility;
using uPLibrary.Networking.M2Mqtt.Exceptions;

using System;
using System.Text;

public class CreateProfile : MonoBehaviour {

    public InputField Field;
    private MqttClient client;
    private const string str_IP = "127.0.0.1";
    private const int int_Port = 1883;
    private const string topic = "database";

    public void AddProfile()
    {
        string player_name = Field.text;
        string str_command = "INSERT INTO players (name) VALUES (\'" + player_name + "\')";
        byte[] command = Encoding.ASCII.GetBytes(str_command);

        // create client instance 
        client = new MqttClient(IPAddress.Parse(str_IP), int_Port, false, null);

        // connect
        string clientId = Guid.NewGuid().ToString();
        client.Connect(clientId);

        //Add Profile
        client.Publish("database", command);
    }
}
