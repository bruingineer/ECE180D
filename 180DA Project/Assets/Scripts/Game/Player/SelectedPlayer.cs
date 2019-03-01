using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SelectedPlayer{
    
    //player info
    public static string name;
    public static int id, games_played, difficulty_ctr;
    public static string suggested_difficulty;
    public static bool laser_training, gesture_training, unscramble_training, trivia_training;
    
    //current game info
    public static string current_difficulty = "easy";
    
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
}
