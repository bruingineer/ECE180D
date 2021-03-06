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
    public float gestures_timer_avg;

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

    private const int N_GAMES_TRAINING = 3;
    private const int N_GAMES_DIFFICULTY = 3;
    private const int UPPER_THRESH = 250;
    private const int LOWER_THRESH = 150;

    //MQTT stuff
    private static MqttClient client;
    private const string str_IP = "127.0.0.1";
    private const int int_Port = 1883;
    private const string topic = "database/games";

    GameData gd;

    //Text objects to display game results and smart training suggestion
    public Text gesture_acc;
    public Text unscramble_acc;
    public Text trivia_acc;
    public Text lives_left_txt;
    public Text training_suggestion;
    public Text score_txt;
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

        //calculate results for current game and update database
        CalculateResults();

        //Query game data for iterative learning model
        QueryDB("Training");
    }

    //Update training suggestion on screen based on iterative learning results
    void Update()
    {
        if (suggestion != null)
            training_suggestion.text = suggestion;

        if (first_suggestion != null)
            temp_text.text = first_suggestion;

        //Once training query and sugestion is determined,
        //perform query for difficulty change 
        if (training_query_done && !difficulty_query_done)
        {
            QueryDB("difficulty");
            difficulty_query_done = true;
        }
    }


    public void CalculateResults()
    {
        List<float> results = SelectedPlayer.GetResults(false);
        float g = results[0];           //gestures acc
        float g_tleft_avg = results[1];
        float u = results[2];           //unscramble acc
        float u_tleft_avg = results[3];
        float t = results[4];           //trivia acc
        float t_tleft_avg = results[5];
        float total_score = results[6];
        bool died = SelectedPlayer.died;
        int lives_left = SelectedPlayer.current_lives_left;


        /////////////////HARDCODED test values///////////////
        //g = 0.77f;
        //u = 0.77f;
        //t = 0.77f;
        //g_tleft_avg = 8;
        //u_tleft_avg = 8;
        //t_tleft_avg = 8;
        //died = false;
        //lives_left = 1;
        //total_score = 50 * (g + u + t + g_tleft_avg / 10 + u_tleft_avg / 10 + t_tleft_avg / 10 + lives_left / 3);
        //total_score += 100;
        ////////////////////////////////////////////////////

        //Populate Results in End Game scene

        if (g == -1) gesture_acc.text += "\tNA";
        else
            gesture_acc.text += string.Format("\t{0}% accuracy, {1}s time left avg" , 
                                                (100 * g).ToString("0.##"), g_tleft_avg.ToString("0.##"));

        if (u == -1) unscramble_acc.text += "\tNA";
        else
            unscramble_acc.text += string.Format("\t{0}% accuracy, {1}s time left avg",
                                            (100 * u).ToString("0.##"), u_tleft_avg.ToString("0.##"));

        if (t == -1) trivia_acc.text += "\tNA";
        else 
            trivia_acc.text += string.Format("\t{0}% accuracy, {1}s time left avg",
                                            (100 * t).ToString("0.##"), t_tleft_avg.ToString("0.##"));

        lives_left_txt.text += lives_left.ToString();
        score_txt.text += total_score.ToString("0.##");

        //Input game data into database
        if (SelectedPlayer.name != null)
            UpdateDatabase(g, u, t, Convert.ToInt32(died), g_tleft_avg, u_tleft_avg, t_tleft_avg, lives_left, total_score);
        }
       
    
    
    void UpdateDatabase(float g, float u, float t, int d, float g_tleft_avg, float u_tleft_avg,
                        float t_tleft_avg, int lives_left, float total_score)
    {
        //Insert game data into db
        string values = string.Format("({0}, {1}, \"{2}\", {3}, {4}, {5}, {6}, {7}, {8}, {9}, {10}, {11})",
                                    SelectedPlayer.id, ++SelectedPlayer.games_played, SelectedPlayer.current_difficulty,
                                    g, u, t, d, g_tleft_avg, u_tleft_avg, t_tleft_avg, lives_left, total_score);

        string str_command = "INSERT INTO games (player, player_game_idx, difficulty, gestures_acc, unscramble_acc," +
                                                " trivia_acc, died, gestures_timer_avg, unscramble_timer_avg, trivia_timer_avg, " +
                                                "lives_left, total_score)" +
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
    
    void QueryDB(string query)
    {
        string str_command;
        //Perform the query for selected player's last three game data for Training Suggestion
        if (query == "Training")
        {
            str_command = string.Format("SELECT * FROM (SELECT * FROM games WHERE player={0}) sub ", SelectedPlayer.id) +
                                                "ORDER BY game_id DESC LIMIT " + N_GAMES_TRAINING.ToString() ;
        }

        //Perform the query for selected player's last five game data for Difficulty Suggesion
        else
        {
            str_command = string.Format("SELECT * FROM (SELECT * FROM games WHERE player={0} AND difficulty=\"{1}\") sub ",
                                        SelectedPlayer.id, SelectedPlayer.suggested_difficulty) +
                                        "ORDER BY game_id DESC LIMIT " + N_GAMES_DIFFICULTY.ToString();
        }

        byte [] command = Encoding.ASCII.GetBytes(str_command);
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
        if (SelectedPlayer.games_played < N_GAMES_TRAINING)
        {
            suggestion = "Keep playing the game to receive smart training tips!";
        }
        else
        {
            //learning model features
            float avg_gesture_acc = 0;
            float avg_unscramble_acc = 0;
            float avg_trivia_acc = 0;
            float avg_g_timeleft = 0;
            float avg_u_timeleft = 0;
            float avg_t_timeleft = 0;
            float total_score_avg = 0;
            int lives_left = 0;

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

                    //gesture stats sum
                    if (game.gestures_acc != -1)
                    {
                        avg_gesture_acc += game.gestures_acc;
                        avg_g_timeleft += game.gestures_timer_avg;
                    }
                    else gesture_ctr--;

                    //unscrambler stats sum
                    if (game.unscramble_acc != -1)
                    {
                        avg_unscramble_acc += game.unscramble_acc;
                        avg_u_timeleft += game.unscramble_timer_avg;
                    }
                    else unscramble_ctr--;

                    //trivia stats sum
                    if (game.trivia_acc != -1)
                    {
                        avg_trivia_acc += game.trivia_acc;
                        avg_t_timeleft += game.trivia_timer_avg;
                    }
                    else trivia_ctr--;

                    lives_left += game.lives_left;
                    total_score_avg += game.total_score;
                }

                //gesture averages
                if (gesture_ctr != 0) {
                    avg_gesture_acc = avg_gesture_acc / gesture_ctr;
                    avg_g_timeleft = avg_g_timeleft / gesture_ctr;
                }
                else avg_gesture_acc = -1;

                //unscramble averages
                if (unscramble_ctr != 0) {
                    avg_unscramble_acc = avg_unscramble_acc / unscramble_ctr;
                    avg_u_timeleft = avg_u_timeleft / unscramble_ctr;
                }
                else avg_unscramble_acc = -1;

                //trivia averages
                if (trivia_ctr != 0) {
                    avg_trivia_acc = avg_trivia_acc / trivia_ctr;
                    avg_t_timeleft = avg_t_timeleft / trivia_ctr;
                }
                else avg_trivia_acc = -1;

                total_score_avg = total_score_avg / gd.count;

                //Debug.Log("avg_gesture_acc " + avg_gesture_acc);
                //Debug.Log("avg_unscramble_acc " + avg_unscramble_acc);
                //Debug.Log("avg_trivia_acc " + avg_trivia_acc);
                //Debug.Log("n_deaths " + n_deaths);
                //Debug.Log("lives left: " + lives_left);

                //First query is for training suggestion (3 games)
                if (!training_query_done)
                {
                    //Performing well in all categories
                    if (avg_gesture_acc >= 0.69 && avg_unscramble_acc >= 0.69 && avg_trivia_acc >= 0.69 && lives_left >= 4
                        && avg_g_timeleft > 3 && avg_u_timeleft > 3 && avg_t_timeleft > 3)
                    {
                        if (SelectedPlayer.suggested_difficulty != "hard")
                            suggestion = "Suggestion: Great Work! Keep it up to unlock the next difficulty!";
                        else
                            suggestion = "Suggestion: Great Work! Keep it up!";
                    }

                    else
                    {
                        suggestion = "Suggestion:\nImprove your performance by playing the following mini-games:";
                        if (lives_left < 4)
                        {
                            suggestion += "\n\tLasers Training";
                        }
                        if (avg_gesture_acc < .69 || avg_g_timeleft <= 3)
                        {
                            suggestion += "\n\tGestures Training";
                        }
                        if (avg_unscramble_acc < .69 || avg_u_timeleft <= 3)
                        {
                            suggestion += "\n\tUnscramble Training";
                        }
                        if (avg_trivia_acc < .69 || avg_t_timeleft <= 3)
                        {
                            suggestion += "\n\tTrivia Training";
                        }
                        first_suggestion = suggestion;
                    }

                    training_query_done = true;
                }

                //Second Query is to determine difficulty eligibility
                else 
                {
                    //Only consider difficulty change when at least N games have
                    //been played in currently suggested difficulty
                    if (SelectedPlayer.difficulty_ctr >= N_GAMES_DIFFICULTY)
                    {
                        Debug.Log("Checking for difficulty change!");
                        //if (n_deaths <= 1 && avg_gesture_acc >= 0.69 && avg_unscramble_acc >= 0.69 && avg_trivia_acc >= 0.69)
                        if (total_score_avg >= UPPER_THRESH)
                            ChangeDifficulty("higher");
                        //else if (n_deaths >= 4 || (avg_gesture_acc <= 0.39 && avg_unscramble_acc <= 0.39 && avg_trivia_acc <= 0.39))
                        else if (total_score_avg < LOWER_THRESH)
                            ChangeDifficulty("lower");
                        else Debug.Log("Difficulty staying the same");
                    }
                }
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
