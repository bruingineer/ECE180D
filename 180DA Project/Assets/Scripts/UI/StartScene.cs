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

[Serializable]
public class PlayerData
{
    public int count;
    public PlayerItem[] items;

    public static PlayerData CreateFromJSON(string jsonString)
    {
        return JsonUtility.FromJson<PlayerData>(jsonString);
    }
}

[Serializable]
public class PlayerItem
{
    public string name;
    public int id;
    public int games_played;
    public string suggested_difficulty;
}

public class StartScene : MonoBehaviour {

    private const string str_IP = "127.0.0.1";
    private const int int_Port = 1883;
    private const string topic = "database/result";
    private byte[] playersQuery = Encoding.ASCII.GetBytes("SELECT * FROM players");
    private bool populated = false;
    public double timer = 0;
    PlayerData pd;
    PlayerItem selectedPlayer;

    //Create a List of new Dropdown options and attach to object
    List<string> m_DropOptions = new List<string>();
    Dropdown m_Dropdown;

    private MqttClient client;

    // Use this for initialization
    void Start () {
        // create client instance 
        client = new MqttClient(IPAddress.Parse(str_IP), int_Port, false, null);

        // register to message received 
        client.MqttMsgPublishReceived += client_MqttMsgPublishReceived;

        string clientId = Guid.NewGuid().ToString();
        client.Connect(clientId);
        client.Subscribe(new string[] { topic }, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });

        //Perform query for player profiles
        client.Publish("database", playersQuery);
    }
	
	// Update is called once per frame
	void Update () {
        //Populate dropdown if profiles were found in database
        if (populated)
        {
            //Fetch the Dropdown GameObject the script is attached to
            m_Dropdown = GetComponent<Dropdown>();
            m_Dropdown.ClearOptions();

            //Add the options from database query
            m_Dropdown.AddOptions(m_DropOptions);
        }

        if (pd != null && pd.count != 0)
        {
            selectedPlayer = pd.items[m_Dropdown.value];
            if (selectedPlayer.name != SelectedPlayer.name)
            {
                SelectedPlayer.name = selectedPlayer.name;
                SelectedPlayer.id = selectedPlayer.id;
                SelectedPlayer.games_played = selectedPlayer.games_played;
                SelectedPlayer.suggested_difficulty = selectedPlayer.suggested_difficulty;
                SelectedPlayer.resetGameStats();
            }
        }
	}

    void client_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
    {
        string playersResult = Encoding.ASCII.GetString(e.Message);
        Debug.Log(playersResult);

        pd = PlayerData.CreateFromJSON(playersResult);
        if (pd == null || pd.count == 0)
        {
            Debug.Log("No Profiles Found!");
            return;
        }

        for(int i=0; i<pd.count; i++)
        {
            m_DropOptions.Add(pd.items[i].name);
        }
        populated = true;
    }
}
