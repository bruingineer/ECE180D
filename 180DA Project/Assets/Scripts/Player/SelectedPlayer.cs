using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SelectedPlayer{
    
    //player info
    public static string name;
    public static int id, games_played, difficulty_ctr;
    public static string suggested_difficulty;
    public static bool laser_training, gesture_training, speech_training;

    //current game info
    public static int current_hits = 0;
    public static float current_gesture_pass = 0;
    public static float current_gesture_fail = 0;
    public static float current_speech_pass = 0;
    public static float current_speech_fail = 0;
    public static int bigLaserctr = 0;
    public static bool died = false;
    public static string current_difficulty = "easy";

    public static void resetGameStats()
    {
        current_hits = 0;
        current_gesture_pass = 0;
        current_gesture_fail = 0;
        current_speech_pass = 0;
        current_speech_fail = 0;
        bigLaserctr = 0;
        died = false;
    }
}
