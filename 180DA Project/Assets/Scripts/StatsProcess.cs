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
public class GameData
{
    public int count;
    public GameItem[] items;

    public static GameData CreateFromJSON(string jsonString)
    {
        return JsonUtility.FromJson<GameData>(jsonString);
    }
}

[Serializable]
public class GameItem
{
    public int game_id;
    public int player;
    public int player_game_idx;
    public float gestures_acc;
    public float speech_acc;
    public bool died;
}

public class StatsProcess : MonoBehaviour
{
    private MqttClient client;
    GameData gd;
    private const string str_IP = "127.0.0.1";
    private const int int_Port = 1883;
    private const string topic = "database/result";

    //Text objects to display game results and smart training suggestion
    public Text gesture_acc;
    public Text speech_acc;
    public Text survived;
    public Text training_suggestion;

    string suggestion;
    

    void UpdateDatabase(float g, float s, int d)
    {
        //Insert game data into db
        string values = string.Format("({0}, {1}, {2}, {3}, {4})",
                                    SelectedPlayer.id, SelectedPlayer.games_played, g, s, d);
        string str_command = "INSERT INTO games (player, player_game_idx, gestures_acc, speech_acc, died) VALUES  " + values;
        byte[] command = Encoding.ASCII.GetBytes(str_command);
        client.Publish("database", command);

        //Update player's games_played field
        SelectedPlayer.games_played++;
        str_command = string.Format("UPDATE players SET games_played = {0} WHERE id = {1}",
                                    SelectedPlayer.games_played, SelectedPlayer.id);
        command = Encoding.ASCII.GetBytes(str_command);
        client.Publish("database", command);
    }

    void PredictTraining()
    {
        if (SelectedPlayer.games_played < 3)
        {
            suggestion = "Keep playing the game to receive smart training tips!";
        }
        else
        {
            float avg_gesture_acc = 0;
            float avg_speech_acc = 0;
            int gesture_ctr = gd.count;
            int speech_ctr = gd.count;
            int n_deaths = 0;

            //Predict which training the player requires most
            if (gd != null && gd.count != 0)
            {
                foreach (GameItem game in gd.items)
                {
                    //If there were no gesture or speech commands in game, do not count toward average
                    if (game.gestures_acc != -1) avg_gesture_acc += game.gestures_acc;
                    else gesture_ctr--;

                    if (game.speech_acc != -1) avg_speech_acc += game.speech_acc;
                    else speech_ctr--;

                    Debug.Log("game.died: " + game.died);
                    if(game.died)
                    {
                        n_deaths++;
                    }
                }

                if (gesture_ctr != 0) avg_gesture_acc = avg_gesture_acc / gesture_ctr;
                else avg_gesture_acc = -1;
                if (speech_ctr != 0) avg_speech_acc = avg_speech_acc / speech_ctr;
                else avg_speech_acc = -1;

                Debug.Log("avg_gesture_acc " + avg_gesture_acc);
                Debug.Log("avg_speech_acc " + avg_speech_acc);
                Debug.Log("n_deaths " + n_deaths);

                if (avg_gesture_acc >= 0.8 && avg_speech_acc >= 0.8 && n_deaths <= 1)
                {
                    suggestion = "Suggestion: Great Work! Consider increasing the difficulty for a challenge!";
                    return;
                }
               

                suggestion = "Suggestion:\n Improve your performance by playing the following mini-games: ";
                if(n_deaths >= 2)
                {
                    suggestion += "Lasers Training";
                }
                if(avg_gesture_acc <= .5)
                {
                    suggestion += ", Gestures Training";
                }
                if (avg_speech_acc <= .5)
                {
                    suggestion += ", Speech Training";
                }

            }
        }
    }

    // Use this for initialization
    void Start()
    {
        // create client instance 
        client = new MqttClient(IPAddress.Parse(str_IP), int_Port, false, null);

        // register to message received 
        client.MqttMsgPublishReceived += client_MqttMsgPublishReceived;

        // connect and subscribe to db results
        string clientId = Guid.NewGuid().ToString();
        client.Connect(clientId);
        client.Subscribe(new string[] { topic }, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });

        float g;
        float s;
        bool died;
        int d;

        //calculate accuracies for the game that just ended and call UpdateDatabase() 
        //then query last 3 games' data to calculate averages and predict training suggestion
        if (SelectedPlayer.name != null)
        {
            Debug.Log("current_gesture_fail: " + SelectedPlayer.current_gesture_fail);
            Debug.Log("current_gesture_pass: " + SelectedPlayer.current_gesture_pass);
            Debug.Log("current_speech_fail: " + SelectedPlayer.current_speech_fail);
            Debug.Log("current_speech_pass: " + SelectedPlayer.current_speech_pass);

            int tot_gestures = (SelectedPlayer.current_gesture_fail + SelectedPlayer.current_gesture_pass);
            int tot_speech = (SelectedPlayer.current_speech_fail + SelectedPlayer.current_speech_pass);

            if (tot_gestures == 0) g = -1;
            else g = SelectedPlayer.current_gesture_pass / tot_gestures;

            if (tot_speech == 0) s = -1;
            else s = SelectedPlayer.current_speech_pass / tot_speech;

            died = SelectedPlayer.died;
            //g = 0.3f;
            //s = 0.88f;
            //died = true;

            gesture_acc.text += ("  " + g);
            speech_acc.text += ("  " + s);
            if (died)
            {
                survived.text += ("  No");
                d = 1;
            }
            else
            {
                survived.text += ("  Yes");
                d = 0;
            }

            UpdateDatabase(g, s, d);

            //Perform the query for selected player's last three game data
            string str_command = string.Format("SELECT * FROM (SELECT * FROM games WHERE player={0}) sub ", SelectedPlayer.id) +
                                             "ORDER BY game_id DESC LIMIT 3";
            byte[] command = Encoding.ASCII.GetBytes(str_command);
            client.Publish("database", command);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (suggestion != null)
        {
            training_suggestion.text = suggestion;
        }
    }

    //MQTT message expected to contain game results as JSON string
    void client_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
    {
        //unpack JSON
        string gamesResult = Encoding.ASCII.GetString(e.Message);
        Debug.Log(gamesResult);
        gd = GameData.CreateFromJSON(gamesResult);

        //Give Smart suggestion and reset game stats for new game
        PredictTraining();
        SelectedPlayer.resetGameStats();
    }
}
