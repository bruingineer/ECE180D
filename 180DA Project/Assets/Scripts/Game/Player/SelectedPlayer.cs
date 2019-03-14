using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SelectedPlayer{
    
    //player info from DB
    public static string name;
    public static int id, games_played, difficulty_ctr;
    public static string suggested_difficulty;
    public static bool laser_training, gesture_training, unscramble_training, trivia_training;

    //current game info
    public static string current_difficulty = "easy";
    public static bool new_score = true;

    //gesture stats
    public static float current_gesture_pass = 0;
    public static float current_gesture_fail = 0;
    public static float current_g_timer_avg = 0; //avg time left on timer for gestures 
    
    //unscrambler stats
    public static float current_unscramble_pass = 0;  
    public static float current_unscramble_fail = 0;
    public static float current_unscramble_timer_avg = 0; //avg time left on timer for unscramble

    //trivia stats
    public static float current_trivia_pass = 0;  
    public static float current_trivia_fail = 0;
    public static float current_trivia_timer_avg = 0; //avg time left on timer for trivia

    //lives stats
    public static bool died = false;
    public static int current_lives_left = 3;

    public static void resetGameStats()
    {
        current_lives_left = 3;
        current_gesture_pass = 0;
        current_gesture_fail = 0;
        current_g_timer_avg = 0;
        current_unscramble_pass = 0;
        current_unscramble_fail = 0;
        current_unscramble_timer_avg = 0;

        current_trivia_pass = 0;
        current_trivia_fail = 0;
        current_trivia_timer_avg = 0;
        died = false;
    }

   public static List<float> GetAverages()
    {
        // averages considered:
        // [g, g_tleft_avg, u, u_tleft_avg, t, t_tleft_avg]
        List<float> averages = new List<float>();

        //Debug.Log("current_gesture_fail: " + SelectedPlayer.current_gesture_fail);
        //Debug.Log("current_gesture_pass: " + SelectedPlayer.current_gesture_pass);
        //Debug.Log("current_unscramble_fail: " + SelectedPlayer.current_unscramble_fail);
        //Debug.Log("current_unscramble_pass: " + SelectedPlayer.current_unscramble_pass);
        //Debug.Log("current_trivia_fail: " + SelectedPlayer.current_trivia_fail);
        //Debug.Log("current_trivia_pass: " + SelectedPlayer.current_trivia_pass);


        float tot_gestures = (current_gesture_fail + current_gesture_pass);
        float tot_unscramble = (current_unscramble_fail + current_unscramble_pass);
        float tot_trivia = (current_trivia_fail + current_trivia_pass);

        //gesture averages
        //out of 50 pts
        if (tot_gestures == 0)
        {
            averages.Add(-1); //g
            averages.Add(-1); //g_tleft_avg
        }
        else
        {
            averages.Add(current_gesture_pass / tot_gestures); //g
            averages.Add(current_g_timer_avg / tot_gestures);  //g_tleft_avg
        }

        //unscramble averages
        //out of 50 points
        if (tot_unscramble == 0)
        {
            averages.Add(-1); //u
            averages.Add(-1); //u_tleft_avg
        }
        else
        {
            averages.Add(current_unscramble_pass / tot_unscramble);      //u
            averages.Add(current_unscramble_timer_avg / tot_unscramble); //u_tleft_avg
        }

        //trivia averages
        //out of 50 points
        if (tot_trivia == 0)
        {
            averages.Add(-1); //t
            averages.Add(-1); //t_tleft_avg
        }
        else
        {
            averages.Add(current_trivia_pass / tot_trivia);      //t
            averages.Add(current_trivia_timer_avg / tot_trivia); //t_tleft_avg
        }

        return averages;
    }

    public static List<float> GetResults(bool score_only)
    {
        // [g, g_tleft_avg, u, u_tleft_avg, t, t_tleft_avg]
        List<float> averages = GetAverages();
        List<float> results = new List<float>();
        float total_score = 0;

        //Add points for averages
        for(int i = 0; i < averages.Count; i++)
        {
            if (averages[i] == -1) continue;
            if (i % 2 == 0) total_score += 50 * averages[i]; //accuracy averages
            else total_score += 5 * averages[i];             //time left averages
        }
        
        //add points for survival
        total_score += (float)current_lives_left / 3 * 50;
        if (current_lives_left != 0) total_score += 100;

        if (score_only)
        {
            results.Add(total_score);
            return results;
        }
        else
        {
            averages.Add(total_score);
            return averages;
        }
    }
}
