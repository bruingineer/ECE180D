using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public static class SelectedPlayer{
    
    //player info
    public static string name;
    public static int id, games_played;

    public static string Name
    {
        get
        {
            return name;
        }
        set
        {
            name = value;
        }
    }

    public static int Id
    {
        get
        {
            return id;
        }
        set
        {
            id = value;
        }
    }

    public static int Games_played
    {
        get
        {
            return games_played;
        }
        set
        {
            games_played = value;
        }
    }

    //current game info
    public static int current_hits = 0;
    public static int current_gesture_pass = 0;
    public static int current_gesture_fail = 0;
    public static int current_speech_pass = 0;
    public static int current_speech_fail = 0;
    public static int bigLaserctr = 0;

    public static void resetGameStats()
    {
        current_hits = 0;
        current_gesture_pass = 0;
        current_gesture_fail = 0;
        current_speech_pass = 0;
        current_speech_fail = 0;
        bigLaserctr = 0;
}
}
