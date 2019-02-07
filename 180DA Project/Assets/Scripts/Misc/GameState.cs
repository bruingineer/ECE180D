using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
using uPLibrary.Networking.M2Mqtt.Utility;
using uPLibrary.Networking.M2Mqtt.Exceptions;

public abstract class GameState_Base : MonoBehaviour {

	// Variables
	public static int numLanes;
	public static int end_row;
	public static List<int> laneNums;

	// Audio
	public static AudioSource m_audio_source;
	protected AudioClip gameLost;
	protected AudioClip gameWon;

	// Objects
	protected Canvas canvas;
	public Text gameOver;
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

		// if (player.playerLives == 1) {
		// 	gameMusic.pitch = 1.25f;
		// } else {
		// 	gameMusic.pitch = 1;
		// } 
	// if (player.playerLives == 1) {
		// 	gameMusic.pitch = 1.25f;
		// } else {
		// 	gameMusic.pitch = 1;
		// } 
	
	void Awake () {
		numLanes = 10;
		end_row = 14;
		m_audio_source = GetComponent<AudioSource>();
		gamePlaying = false;
		canvas = GameObject.Find("Canvas").GetComponent<Canvas>();
		StartCoroutine(StartGame());
	}

	private IEnumerator StartGame()
	{
		// fix to set up first and then start after
		yield return StartCoroutine(GameTimer());
		gamePlaying = true;
		SetUp();
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
		m_audio_source.Play();
	}

	public void RetryLevel()
	{
		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
	}

	public void LoadGameMenu()
	{
		SceneManager.LoadScene(6);
	}
}

public abstract class GameState_with_Player : GameState_Base {
	public GameObject playerExplosion;
	private GameObject player;
	private bool handledPlayer;

	protected override void SetUp()
	{
		handledPlayer = false;
		player = GameObject.Find("Player");
		InitializeLaneList();
		SetUp_Events_Obstacles();
	}

	protected abstract void SetUp_Events_Obstacles();
	private void InitializeLaneList() {
		laneNums = new List<int>();
		for (int i = 1; i <= numLanes; i++)
			laneNums.Add(i);
	}

	void Update()
	{
		if (!handledPlayer && gamePlaying)
		{
			HandlePlayerWin();
			HandlePlayerDied();
		}
	}

	private void HandlePlayerWin() 
	{
		if (player.transform.position.y == (end_row - 0.5f))
		{
			handledPlayer = true;
			gamePlaying = false;
			StartCoroutine(PlayerWonCoroutine());
		}
	}

	private void HandlePlayerDied() 
	{
		if (player.GetComponent<Player>().isDead) {
			handledPlayer = true;
			gamePlaying = false;
			SelectedPlayer.died = true;
			StartCoroutine(PlayerDiedCoroutine());
		}
	}

	IEnumerator PlayerWonCoroutine()
	{
		// gameOver.text = "You Win!";
		Instantiate(gameOver, canvas.transform);
		PlayClip(gameWon);
		yield return new WaitForSeconds(gameWon.length);
		// handle won
	}

	IEnumerator PlayerDiedCoroutine() 
	{
		// gameOver.text = "You Lost!";
		Instantiate(gameOver, canvas.transform);
		ParticleSystem explosion = Instantiate(playerExplosion, player.transform.position, Quaternion.identity).GetComponent<ParticleSystem>();
		Destroy(player);
		PlayClip(gameLost);
		float explosionDuration = gameLost.length;
		var main = explosion.main;
		main.duration = explosionDuration;
		explosion.Play();
		yield return new WaitForSeconds(explosionDuration);
		// handle died
	}
}