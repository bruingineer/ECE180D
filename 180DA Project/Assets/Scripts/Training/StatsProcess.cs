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
    //MQTT stuff
    private MqttClient client;
    GameData gd;
    private const string str_IP = "127.0.0.1";
    private const int int_Port = 1883;
    private const string topic = "database/games";

    //Text objects to display game results and smart training suggestion
    public Text gesture_acc;
    public Text speech_acc;
    public Text survived;
    public Text training_suggestion;
    string suggestion;

    //Capture first training suggestion before difficulty change suggestion replaces it
    //used to fix dropdown menu not populating
    public Text temp_text; 
    string first_suggestion;

    bool training_query_done = false;
    bool difficulty_query_done = false;

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

        //calculate results for current game and update database, then reset stats
        CalculateResults();
        SelectedPlayer.resetGameStats();

        //Query game data for iterative learning model
        QueryDB();
    }
    

    //Update training suggestion on screen based on iterative learning results
    void Update()
    {
        if (suggestion != null)
        {
            training_suggestion.text = suggestion;
        }

        if (first_suggestion != null)
        {
            temp_text.text = first_suggestion;
        }
    }


    void CalculateResults()
    {
        //current game stats to push to database
        float g;
        float s;
        bool died;
        int d;

        //calculate accuracies for the game that just ended and call UpdateDatabase() 
        if (SelectedPlayer.name != null)
        {
            //Debug.Log("current_gesture_fail: " + SelectedPlayer.current_gesture_fail);
            //Debug.Log("current_gesture_pass: " + SelectedPlayer.current_gesture_pass);
            //Debug.Log("current_speech_fail: " + SelectedPlayer.current_speech_fail);
            //Debug.Log("current_speech_pass: " + SelectedPlayer.current_speech_pass);

            float tot_gestures = (SelectedPlayer.current_gesture_fail + SelectedPlayer.current_gesture_pass);
            float tot_speech = (SelectedPlayer.current_speech_fail + SelectedPlayer.current_speech_pass);

            if (tot_gestures == 0) g = -1;  //-1 represents 'NULL'
            else g = SelectedPlayer.current_gesture_pass / tot_gestures;

            if (tot_speech == 0) s = -1;
            else s = SelectedPlayer.current_speech_pass / tot_speech;

            died = SelectedPlayer.died;

            /////////////////HARDCODED test values///////////////
            //g = 0.77f;
            //s = 0.77f;
            //died = false;
            ////////////////////////////////////////////////////

            gesture_acc.text += ("  " + g.ToString("0.##"));
            speech_acc.text += ("  " + s.ToString("0.##"));
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

            //Input game data into database
            UpdateDatabase(g, s, d);
        }
    }

    void UpdateDatabase(float g, float s, int d)
    {
        //Insert game data into db
        string values = string.Format("({0}, {1}, \"{2}\", {3}, {4}, {5})",
                                    SelectedPlayer.id, ++SelectedPlayer.games_played, SelectedPlayer.current_difficulty, g, s, d);
        string str_command = "INSERT INTO games (player, player_game_idx, difficulty, gestures_acc, speech_acc, died) VALUES  " + values;
        byte[] command = Encoding.ASCII.GetBytes(str_command);
        client.Publish("database", command);

        //Update player's games_played field and difficulty_ctr
        if (SelectedPlayer.suggested_difficulty == SelectedPlayer.current_difficulty)
        {
            SelectedPlayer.difficulty_ctr++;
        }

        str_command = string.Format("UPDATE players SET games_played={0}, difficulty_ctr={1} WHERE id = {2}",
                                    SelectedPlayer.games_played, SelectedPlayer.difficulty_ctr, SelectedPlayer.id);
        command = Encoding.ASCII.GetBytes(str_command);
        client.Publish("database", command);
        
    }

    void QueryDB()
    {
        //Perform the query for selected player's last three game data for Training Suggestion
        string str_command = string.Format("SELECT * FROM (SELECT * FROM games WHERE player={0}) sub ", SelectedPlayer.id) +
                                            "ORDER BY game_id DESC LIMIT 3";
        byte[] command = Encoding.ASCII.GetBytes(str_command);
        client.Publish("database", command);

        //Perform the query for selected player's last five game data for Difficulty Suggesion
        str_command = string.Format("SELECT * FROM (SELECT * FROM games WHERE player={0} AND difficulty=\"{1}\") sub ",
                                    SelectedPlayer.id, SelectedPlayer.suggested_difficulty) +
                                    "ORDER BY game_id DESC LIMIT 5";
        command = Encoding.ASCII.GetBytes(str_command);
        client.Publish("database", command);
    }

    void ChangeDifficulty(string direction)
    {
        string current = SelectedPlayer.suggested_difficulty;
        string updated = current;
        if (current == "easy")
        {
            if (direction == "higher") updated = "medium";
        }
        else if (current == "medium")
        {
            if (direction == "higher") updated = "hard";
            if (direction == "lower") updated = "easy";
        }
        else if (current == "hard")
        {
            if (direction == "lower") updated = "medium";
        }

        //Update database if a difficulty change has been allowed
        if (updated != current)
        {
            Debug.Log("Updating player difficulty to :" + updated);
            string str_command = string.Format("UPDATE players SET suggested_difficulty=\"{0}\" WHERE id={1}",
                                        updated, SelectedPlayer.id);
            byte[] command = Encoding.ASCII.GetBytes(str_command);
            client.Publish("database", command);

            SelectedPlayer.suggested_difficulty = updated;
            SelectedPlayer.difficulty_ctr = 0;

            if (direction == "higher") suggestion = "You have unlocked " + updated + " difficulty. Keep it up!";
            else if (direction == "lower") suggestion = "Uh-oh, you need to go back to " + updated + " difficulty. Try training!";

            str_command = string.Format("UPDATE players SET difficulty_ctr = {0} WHERE id = {1}",
                                    SelectedPlayer.difficulty_ctr, SelectedPlayer.id);
            command = Encoding.ASCII.GetBytes(str_command);
            client.Publish("database", command);
        }

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

            //iterative learning 
            if (gd != null && gd.count != 0)
            {   
                //calculate averages over queried games 
                foreach (GameItem game in gd.items)
                {
                    //If there were no gesture or speech commands in game, do not count toward average
                    if (game.gestures_acc != -1) avg_gesture_acc += game.gestures_acc;
                    else gesture_ctr--;

                    if (game.speech_acc != -1) avg_speech_acc += game.speech_acc;
                    else speech_ctr--;
                    
                    if (game.died) n_deaths++;
                }

                if (gesture_ctr != 0) avg_gesture_acc = avg_gesture_acc / gesture_ctr;
                else avg_gesture_acc = -1;
                if (speech_ctr != 0) avg_speech_acc = avg_speech_acc / speech_ctr;
                else avg_speech_acc = -1;

                //Debug.Log("avg_gesture_acc " + avg_gesture_acc);
                //Debug.Log("avg_speech_acc " + avg_speech_acc);
                //Debug.Log("n_deaths " + n_deaths);

                //First query will be for training suggestion
                if (!training_query_done)
                {
                    if (avg_gesture_acc >= 0.69 && avg_speech_acc >= 0.69 && n_deaths < 2)
                    {
                        if (SelectedPlayer.suggested_difficulty != "hard")
                            suggestion = "Suggestion: Great Work! Keep it up to unlock the next difficulty!";
                        else
                            suggestion = "Suggestion: Great Work! Keep it up!";
                        training_query_done = true;
                        return;
                    }

                    suggestion = "Suggestion:\nImprove your performance by playing the following mini-games:";
                    if (n_deaths >= 2)
                    {
                        suggestion += "\n\tLasers Training";
                    }
                    if (avg_gesture_acc < .69)
                    {
                        suggestion += "\n\tGestures Training";
                    }
                    if (avg_speech_acc < .69)
                    {
                        suggestion += "\n\tSpeech Training";
                    }
                    training_query_done = true;
                    first_suggestion = suggestion;
                    return;
                }

                //Second Query is to determine difficulty eligibility
                //Only consider difficulty change when at least 5 games have
                //been played in currently suggested difficulty
                else if (SelectedPlayer.difficulty_ctr >= 5)
                {
                    Debug.Log("Checking for difficulty change!");
                    if (n_deaths <= 1 && avg_gesture_acc >= 0.69 && avg_speech_acc >= 0.69) { ChangeDifficulty("higher");}
                    else if (n_deaths >= 4 || (avg_gesture_acc <= 0.39 && avg_speech_acc <= 0.39)) { ChangeDifficulty("lower");}
                    else Debug.Log("Difficulty staying the same");
                }
                difficulty_query_done = true;
                
            }
        }
    }

    //MQTT message expected to contain game results as JSON string
    void client_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
    {
        string gamesResult = Encoding.ASCII.GetString(e.Message);
        if (gamesResult.Contains("game_id"))
        {
            //Unpack JSON
            Debug.Log(gamesResult);
            gd = GameData.CreateFromJSON(gamesResult);
        
            //Call iterative learning fxn once game data is loaded
            PredictTraining();
        }

        if (training_query_done && difficulty_query_done) client.Disconnect();
    }
}
