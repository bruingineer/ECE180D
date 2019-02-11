﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
using uPLibrary.Networking.M2Mqtt.Utility;
using uPLibrary.Networking.M2Mqtt.Exceptions;
using TMPro;

public abstract class GameState_Base : MonoBehaviour {

	// Variables
	public static int numLanes;
	public static int end_row;
	public static List<int> laneNums;

	// Audio
	// fix later, by having an audio source for objects that make noise
	public AudioSource gameMusic;
	public static AudioSource m_audio_source;
	public AudioClip gameLost;
	public AudioClip gameWon;

	// Objects
	protected Canvas canvas;
	public Text result;
	public Text countdown;
	public static bool gamePlaying;
	
	// // change for minigame
	// GameObject retry;
	// GameObject gameMenu;
	// retry = GameObject.FindGameObjectWithTag("Retry").GetComponent<Text>();
	// gameMenu = GameObject.FindGameObjectWithTag("Menu").GetComponent<Text>();
	// if (SceneManager.GetActiveScene().buildIndex == 0)
		// 	SceneManager.LoadScene(2);
		// else 
		// {
		// 	retry.SetActive(true);
		// 	gameMenu.SetActive(true);
		// }

	void Awake () {
		numLanes = 10;
		end_row = 14;
		m_audio_source = GetComponent<AudioSource>();
		gamePlaying = false;
		canvas = GameObject.Find("Canvas").GetComponent<Canvas>();
		SetUp();
		StartCoroutine(StartGame());
	}

	protected abstract void SetUp_Events_Obstacles();

	private IEnumerator StartGame()
	{
		yield return StartCoroutine(GameTimer());
		gamePlaying = true;
		SetUp_Events_Obstacles();
	}

	protected abstract void SetUp();	

	public static void PlayClip(AudioClip clip) {
		m_audio_source.PlayOneShot(clip);
	}

	public IEnumerator GameTimer() 
	{
		Text countdownText = countdown.GetComponent<Text>();
		float duration = 4f;
      	while(duration >= 0)
      	{
			duration -= Time.deltaTime;
			int integer = (int)duration;
			if (integer >= 1)
				countdownText.text = integer.ToString();
			else
				countdownText.text = "Start!";
          	yield return null;
      	}
		Destroy(countdown);
		gameMusic.Play();
	}

	public void RetryLevel()
	{
		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
	}


	public void LoadGameMenu()
	{
		SceneManager.LoadScene(5);
	}

	public void LoadStatsMenu()
	{
		SceneManager.LoadScene(2);
	}
}

public abstract class GameState_with_Player : GameState_Base {
	public GameObject playerExplosion;
	private Player player;
	private bool handledPlayer;
	public List<GameObject> playerLives;

	protected override void SetUp()
	{
		handledPlayer = false;
		player = (Resources.Load("Prefabs/Player/Player") as GameObject).GetComponent<Player>();
		player = Instantiate(player, new Vector3(numLanes / 2 + 0.5f, 0.5f), Quaternion.identity);
		InitializeLaneList();
	}

	private void InitializeLaneList() {
		laneNums = new List<int>();
		for (int i = 0; i < numLanes; i++)
			laneNums.Add(i);
	}

	// change later
	public void ChangePitch(int playerLives)
	{
		gameMusic.pitch = playerLives > 1 ? 1 : 1.25f;
	}

	void Update()
	{
		if (!handledPlayer && gamePlaying)
		{
			HandlePlayerWin();
			HandlePlayerDied();
		}
	}

	public void RemoveLife(int curLives)
	{
		playerLives[curLives - 1].SetActive(false);
		ChangePitch(curLives);
	}

	private void HandlePlayerWin() 
	{
		if (player.transform.position.y == (end_row - 0.5f))
		{
			handledPlayer = true;
			gamePlaying = false;
			result.text = "You win!";
			PlayClip(gameWon);
			StartCoroutine(PlayerWonCoroutine());
		}
	}

	private void HandlePlayerDied() 
	{
		if (player.isDead) {
			handledPlayer = true;
			gamePlaying = false;
			SelectedPlayer.died = true;
			result.text = "Game Over!";
			ParticleSystem explosion = Instantiate(playerExplosion, player.transform.position, Quaternion.identity).GetComponent<ParticleSystem>();
			Destroy(player.gameObject);
			PlayClip(gameLost);
			float explosionDuration = gameLost.length;
			var main = explosion.main;
			main.duration = explosionDuration;
			explosion.Play();
			StartCoroutine(PlayerDiedCoroutine(explosionDuration));
		}
	}

	IEnumerator PlayerWonCoroutine()
	{
		yield return new WaitForSeconds(gameWon.length);
		LoadStatsMenu();
	}

	IEnumerator PlayerDiedCoroutine(float explosionDuration) 
	{
		yield return new WaitForSeconds(explosionDuration);
		LoadStatsMenu();
	}
}