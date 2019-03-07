﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Multiplayer_Controller : MonoBehaviour
{

    // Objects
    public static MultiplayerClient multiplayerClient;
    public static GameObject connectedButton;
    public static GameObject readyButton;
    public static Multiplayer_Controller instance;

    // MQTT 
    public static string serverTopic;
    public static string serverConnectedTopic = "server/player_connected";
    public static string playerConnectionTopic;
    public static string challengeTopic;
    public static string gameStateTopic = "game/state";
    public static string winnerTopic;
    public static string playerHeader;
    public static string wonMessage = "I WON";
    private int clientID;

    // States
    public static bool playerConnected;
    public static bool connectButtonPressed;
    private bool connectButtonPressedOnce;

    // reset these after game ends
    public static bool gameStarted;
    public static bool readyButtonPressed;
    public static bool lost;
    public static bool won;
    private bool handledConnected;
    private bool gamePlayedOnce;
    private bool handledGameStarted;


    // Use this for initialization
    void Awake()
    {
        instance = this;
        connectedButton = GameObject.Find("Connect Button");
        readyButton = GameObject.Find("Ready Button");
        gamePlayedOnce = false;
        ResetBools();
    }


    // Update is called once per frame
    void Update()
    {
        if (playerConnected && !handledConnected)
            Connected();
        if (gameStarted && !handledGameStarted)
            StartGame();
    }

    private void ResetBools()
    {
        connectButtonPressed = false;
        readyButtonPressed = false;
        won = false;
        lost = false;
        gameStarted = false;
        handledConnected = false;
        handledGameStarted = false;

    }

    public void Connect()
    {
        if (!connectButtonPressed)
        {
            connectButtonPressed = true;
            if (!connectButtonPressedOnce)
            {
                connectButtonPressedOnce = true;
                clientID = Random.Range(0, 1000000);
                serverTopic = "server/" + clientID.ToString();
                multiplayerClient = new MultiplayerClient(serverTopic);
            }
            Debug.Log("Connecting to server...");
            connectedButton.transform.Find("Text").GetComponent<Text>().text = "Connecting...";
            multiplayerClient.SendMessage(serverConnectedTopic, clientID.ToString());
            Invoke("ResetConnectButton", 4f);
        }
    }

    private void Connected()
    {
        handledConnected = true;
        challengeTopic = playerHeader + "challenge";
        winnerTopic = playerHeader + "winner_notification";
        playerConnectionTopic = playerHeader + "connection_status";
        Debug.Log("Player connected to server...");
        GetComponent<Obstacles_Multiplayer>().SetUpClient(playerHeader);
        GetComponent<PlayerEvents_Multiplayer>().SetUpClient(playerHeader);
        connectedButton.SetActive(false);
    }

    public void Ready()
    {
        if (!readyButtonPressed)
        {
            readyButtonPressed = true;
            Debug.Log("Player ready and waiting...");
            readyButton.transform.Find("Text").GetComponent<Text>().text = "Waiting to play...";
            if (!gamePlayedOnce)
            {
                multiplayerClient.Subscribe(new string[] { gameStateTopic, challengeTopic, winnerTopic });
                gamePlayedOnce = true;
            }
            multiplayerClient.SendMessage(playerConnectionTopic, "ready");
        }
    }

    public void ResetReadyButton()
    {
        readyButtonPressed = false;
        Multiplayer_Controller.readyButton.transform.Find("Text").GetComponent<Text>().text = "Ready";
        readyButton.SetActive(true);
        multiplayerClient.ResetStart();
        instance.ResetGame();
    }

    private void ResetGame()
    {
        GameState_Base.gameMusic.Stop();
        GetComponent<GameState_Multiplayer>().ResetGameState();
        ResetBools();
    }

    public void ResetConnectButton()
    {
        if (!playerConnected)
        {
            connectButtonPressed = false;
            connectedButton.transform.Find("Text").GetComponent<Text>().text = "Connect";
        }
    }

    private void StartGame()
    {
        handledGameStarted = true;
        readyButton.SetActive(false);
        GameState_Base.StartGame();
    }

}

