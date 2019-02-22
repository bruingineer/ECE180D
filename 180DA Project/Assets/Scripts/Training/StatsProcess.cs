﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Net;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
using uPLibrary.Networking.M2Mqtt.Utility;
using uPLibrary.Networking.M2Mqtt.Exceptions;

using System;
using System.Text;

//Classes to populate game data upon database query
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
    //player data
    public int game_id;
    public int player;
    public int player_game_idx;

    //performance data
    public float gestures_acc;
    public float gesture_timer_avg;

    public float unscramble_acc;
    public float unscramble_timer_avg;

    public float trivia_acc;
    public float trivia_timer_avg;

    public bool died;
    public int lives_left;
    public float total_score;
}

public class StatsProcess : MonoBehaviour
{
    //MQTT stuff
    private static MqttClient client;
    GameData gd;
    private const string str_IP = "127.0.0.1";
    private const int int_Port = 1883;
    private const string topic = "database/games";

    //Text objects to display game results and smart training suggestion
    public Text gesture_acc;
    public Text unscramble_acc;
    public Text trivia_acc;
    public Text survived;
    public Text training_suggestion;
    string suggestion;

    //Capture first 'training suggestion' before 'difficulty change suggestion' replaces it
    //used to fix dropdown menu not populating
    public Text temp_text; 
    string first_suggestion;

    bool training_query_done = false;
    bool difficulty_query_done = false;
    
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
        float g; //gestures acc
        float u; //unscramble acc
        float t; //trivia acc
        float g_tleft_avg;
        float u_tleft_avg;
        float t_tleft_avg;
        bool died;
        int lives_left;

        //calculate accuracies for the game that just ended and call UpdateDatabase() 
        if (SelectedPlayer.name != null)
        {
            //Debug.Log("current_gesture_fail: " + SelectedPlayer.current_gesture_fail);
            //Debug.Log("current_gesture_pass: " + SelectedPlayer.current_gesture_pass);
            //Debug.Log("current_unscramble_fail: " + SelectedPlayer.current_unscramble_fail);
            //Debug.Log("current_unscramble_pass: " + SelectedPlayer.current_unscramble_pass);
            //Debug.Log("current_trivia_fail: " + SelectedPlayer.current_trivia_fail);
            //Debug.Log("current_trivia_pass: " + SelectedPlayer.current_trivia_pass);

            float tot_gestures = (SelectedPlayer.current_gesture_fail + SelectedPlayer.current_gesture_pass);
            float tot_unscramble = (SelectedPlayer.current_unscramble_fail + SelectedPlayer.current_unscramble_pass);
            float tot_trivia = (SelectedPlayer.current_trivia_fail + SelectedPlayer.current_trivia_pass);

            //gesture averages
            if (tot_gestures == 0) {
                g = -1;
                g_tleft_avg = -1;
            }
            else {
                g = SelectedPlayer.current_gesture_pass / tot_gestures;
                g_tleft_avg = SelectedPlayer.current_g_timer_avg / tot_gestures;
            }

            //unscramble averages
            if (tot_unscramble == 0) {
                u = -1;
                u_tleft_avg = -1;
            }
            else {
                u = SelectedPlayer.current_unscramble_pass / tot_unscramble;
                u_tleft_avg = SelectedPlayer.current_unscramble_timer_avg / tot_unscramble;
            }

            //trivia averages
            if (tot_trivia == 0) {
                t = -1;
                t_tleft_avg = -1;
            }
            else {
                t = SelectedPlayer.current_trivia_pass / tot_trivia;
                t_tleft_avg = SelectedPlayer.current_trivia_timer_avg / tot_trivia;
            }

            Debug.Log("g_tleft_avg" + g_tleft_avg);
            Debug.Log("u_tleft_avg" + u_tleft_avg);
            Debug.Log("t_tleft_avg" + t_tleft_avg);

            died = SelectedPlayer.died;
            lives_left = SelectedPlayer.current_lives_left;
            /////////////////HARDCODED test values///////////////
            //g = 0.77f;
            //u = 0.77f;
            //t = 0.77f;
            //died = false;
            ////////////////////////////////////////////////////

            gesture_acc.text += ("  " + g.ToString("0.##"));
            unscramble_acc.text += ("  " + u.ToString("0.##"));
            trivia_acc.text += ("  " + t.ToString("0.##"));
            if (died) survived.text += ("  No");
            else survived.text += ("  Yes");
               

            //Input game data into database
            UpdateDatabase(g, u, t, Convert.ToInt32(died), g_tleft_avg, u_tleft_avg, t_tleft_avg, lives_left);
        }
    }

    //TODO: test if boolean d works
    void UpdateDatabase(float g, float u, float t, int d, float g_tleft_avg, float u_tleft_avg, 
                        float t_tleft_avg, int lives_left)
    {
        //Insert game data into db
        string values = string.Format("({0}, {1}, \"{2}\", {3}, {4}, {5}, {6}, {7}, {8}, {9})",
                                    SelectedPlayer.id, ++SelectedPlayer.games_played, SelectedPlayer.current_difficulty,
                                    g,u,t,d, u_tleft_avg, t_tleft_avg, lives_left);

        string str_command = "INSERT INTO games (player, player_game_idx, difficulty, gestures_acc, unscramble_acc," +
                                                " trivia_acc, died, unscramble_timer_avg, trivia_timer_avg, lives_left)" +
                             "VALUES  " + values;
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
    
    public static void CheckIfTrainingComplete(string training)
    {
        bool selectedTrainingComplete;
        switch(training)
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
        
        string str_command = string.Format("UPDATE players SET {1}=1 WHERE id = {2}",
            selectedTrainingComplete, SelectedPlayer.id);
        byte[] command = Encoding.ASCII.GetBytes(str_command);
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
            //update local variables
            SelectedPlayer.suggested_difficulty = updated;
            SelectedPlayer.difficulty_ctr = 0;

            //push new difficulty and reset ctr to database
            string str_command = string.Format("UPDATE players SET suggested_difficulty=\"{0}\", difficulty_ctr={1} WHERE id={2}",
                                        updated, SelectedPlayer.difficulty_ctr, SelectedPlayer.id);
            byte[] command = Encoding.ASCII.GetBytes(str_command);
            client.Publish("database", command);

            //Replace training suggestion
            if (direction == "higher") suggestion = "You have unlocked " + updated + " difficulty. Keep it up!";
            else if (direction == "lower") suggestion = "Uh-oh, you need to go back to " + updated + " difficulty. Try training!";
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
            //learning model features
            float avg_gesture_acc = 0;
            float avg_g_timeleft = 0;
            float avg_u_timeleft = 0;
            float avg_t_timeleft = 0;
            float avg_unscramble_acc = 0;
            float avg_trivia_acc = 0;
            int lives_left = 0;
            int n_deaths = 0;

            // ctrs tracks # games that contained the type of event 
            int gesture_ctr, unscramble_ctr, trivia_ctr;
            gesture_ctr = unscramble_ctr = trivia_ctr = gd.count;
            
            //iterative learning 
            if (gd != null && gd.count != 0)
            {   
                //calculate averages over queried games 
                foreach (GameItem game in gd.items)
                {
                    //Add accuracies from queried games
                    //if there were no events of a type, do not count toward average

                    if (game.gestures_acc != -1)
                    {
                        avg_gesture_acc += game.gestures_acc;
                        avg_g_timeleft += game.gesture_timer_avg;
                    }
                    else gesture_ctr--;

                    if (game.unscramble_acc != -1)
                    {
                        avg_unscramble_acc += game.unscramble_acc;
                        avg_u_timeleft += game.unscramble_timer_avg;
                    }
                    else unscramble_ctr--;

                    if (game.trivia_acc != -1)
                    {
                        avg_trivia_acc += game.trivia_acc;
                        avg_t_timeleft += game.trivia_timer_avg;
                    }
                    else trivia_ctr--;

                    lives_left += game.lives_left;
                    if (game.died) n_deaths++;
                }

                if (gesture_ctr != 0) avg_gesture_acc = avg_gesture_acc / gesture_ctr;
                else avg_gesture_acc = -1;

                if (unscramble_ctr != 0) avg_unscramble_acc = avg_unscramble_acc / unscramble_ctr;
                else avg_unscramble_acc = -1;

                if (trivia_ctr != 0) avg_trivia_acc = avg_trivia_acc / trivia_ctr;
                else avg_trivia_acc = -1;

                //Debug.Log("avg_gesture_acc " + avg_gesture_acc);
                //Debug.Log("avg_unscramble_acc " + avg_unscramble_acc);
                //Debug.Log("avg_trivia_acc " + avg_trivia_acc);
                //Debug.Log("n_deaths " + n_deaths);
                //Debug.Log("lives left: " + lives_left);

                //First query is for training suggestion (3 games)
                if (!training_query_done)
                {
                    training_query_done = true;

                    //Performing well in all categories
                    if (avg_gesture_acc >= 0.69 && avg_unscramble_acc >= 0.69 && avg_trivia_acc >= 0.69 && n_deaths < 2)
                    {
                        if (SelectedPlayer.suggested_difficulty != "hard")
                            suggestion = "Suggestion: Great Work! Keep it up to unlock the next difficulty!";
                        else
                            suggestion = "Suggestion: Great Work! Keep it up!";
                    }   

                    else
                    {
                        suggestion = "Suggestion:\nImprove your performance by playing the following mini-games:";
                        if (n_deaths >= 2)
                        {
                            suggestion += "\n\tLasers Training";
                        }
                        if (avg_gesture_acc < .69)
                        {
                            suggestion += "\n\tGestures Training";
                        }
                        if (avg_unscramble_acc < .69)
                        {
                            suggestion += "\n\tUnscramble Training";
                        }
                        if (avg_trivia_acc < .69)
                        {
                            suggestion += "\n\tTrivia Training";
                        }
                        first_suggestion = suggestion;
                    }
                }

                //Second Query is to determine difficulty eligibility
                //Only consider difficulty change when at least 5 games have
                //been played in currently suggested difficulty
                else if (SelectedPlayer.difficulty_ctr >= 5)
                {
                    Debug.Log("Checking for difficulty change!");
                    if (n_deaths <= 1 && avg_gesture_acc >= 0.69 && avg_unscramble_acc >= 0.69 && avg_trivia_acc >= 0.69)
                         ChangeDifficulty("higher");
                    else if (n_deaths >= 4 || (avg_gesture_acc <= 0.39 && avg_unscramble_acc <= 0.39 && avg_trivia_acc <= 0.39))
                         ChangeDifficulty("lower");
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
        
            //Call iterative learning func. once game data is loaded
            PredictTraining();
        }

        if (training_query_done && difficulty_query_done) client.Disconnect();
    }
}
