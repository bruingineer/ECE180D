﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Multiplayer_Controller : MonoBehaviour {

    // Objects
    public static MultiplayerClient multiplayerClient;
    public static string serverTopic;
    private string playerConnectedTopic = "server/player_connected";
    public static string playerHeader;
    public static GameObject connectedButton;
    public static GameObject readyButton;
    public static bool playerConnected;
    public static string challengeTopic;
    public static string gameStateTopic = "game/state";
    public static string winnerTopic;
    public static bool gameStarted = false;
    public static Multiplayer_Controller instance;

    // Use this for initialization
    void Awake () {
        instance = this;
        connectedButton = GameObject.Find("Connect Button");
        readyButton = GameObject.Find("Ready Button");
    }
    
    
    // Update is called once per frame
    void Update () {
        if (playerConnected)
            Connected();
        if (gameStarted)
            StartGame();
    }

    public void Connect()
    {
        Debug.Log("Connecting to server...");
        connectedButton.transform.Find("Text").GetComponent<Text>().text = "Connecting...";
        int clientNumber_int = Random.Range(0, 1000000);
        string clientNumber_str = clientNumber_int.ToString();
        serverTopic = "server/" + clientNumber_str;
        multiplayerClient = new MultiplayerClient(serverTopic);
        multiplayerClient.SendMessage(playerConnectedTopic, clientNumber_str);
    }

    private void Connected()
    {
        playerConnected = false;
        challengeTopic = playerHeader + "challenge";
        winnerTopic = playerHeader + "winner";
        Debug.Log("Player connected to server...");
        connectedButton.SetActive(false);
    }

    public void Ready()
    {
        Debug.Log("Player ready and waiting...");
        readyButton.transform.Find("Text").GetComponent<Text>().text = "Waiting to play...";
        multiplayerClient.Subscribe(new string[] {gameStateTopic, challengeTopic, winnerTopic});
        
        multiplayerClient.SendMessage(playerHeader + "connection_status", "ready");
    }

    private void StartGame()
    {
        gameStarted = false;
        readyButton.SetActive(false);
        GameState_Base.StartGame();
    }
}

