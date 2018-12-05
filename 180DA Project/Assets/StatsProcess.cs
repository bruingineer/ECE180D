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

public class StatsProcessOld : MonoBehaviour {

    private MqttClient client;
    private const string str_IP = "127.0.0.1";
    private const int int_Port = 1883;
    private const string topic = "database";

    public Text gesture_acc;
    public Text speech_acc;
    public Text n_hits;
    float g;
    float s;
    float n;

	// Use this for initialization
	void Start () {
        //g = SelectedPlayer.current_gesture_pass / (SelectedPlayer.current_gesture_fail + SelectedPlayer.current_gesture_pass);
        //s = SelectedPlayer.current_speech_pass / (SelectedPlayer.current_speech_fail + SelectedPlayer.current_speech_pass);
        //n = SelectedPlayer.current_hits; 
        g = 0.5f;
        s = 0.88f;
        n = 10f;

        gesture_acc.text += ("  " + g);
        speech_acc.text += ("  " + s);
        n_hits.text += ("  " + n);

        int game_idx = SelectedPlayer.games_played + 1;
        SelectedPlayer.games_played++;

        string values = String.Format("(\'{0}\', \'{0}\', \'{0}\', \'{0}\', \'{0}\' )",
                                    SelectedPlayer.id, game_idx, g, s, n);
        //string str_command = "INSERT INTO games (player, player_game_idx, gestures_acc, speech_acc, n_hits)
        //                                  VALUES  ";
        //byte[] command = Encoding.ASCII.GetBytes(str_command);
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
