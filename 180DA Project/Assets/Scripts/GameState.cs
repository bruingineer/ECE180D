using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
using uPLibrary.Networking.M2Mqtt.Utility;
using uPLibrary.Networking.M2Mqtt.Exceptions;

public class GameState : MonoBehaviour {

	public static int numLanes;
	public static int middle_lane;
	public GameObject playerObject;
	private Player player;
	public Canvas canvas;
	public static int end_column;
	private static AudioSource m_audio_source;
	public GameObject gameOver;
	public GameObject countdown;
	public AudioSource gameMusic;
	public static bool gamePlaying;
	public bool handledPlayer;
	public GameObject playerExplosion;
	public AudioClip playerLost;
	public AudioClip playerWon;
	public static bool stopPlaying;
	public static bool gameWon;
	public static List<int> laneNums;
	public static MqttClient client;
	public static string str_IP = "127.0.0.1";
    public static int int_Port = 1883;
	GameObject retry;
	GameObject gameMenu;
	
	void Awake () {
		numLanes = 10;
		middle_lane = numLanes/2;
		end_column = 20;
		m_audio_source = GetComponent<AudioSource>();
		gamePlaying = false;
		handledPlayer = false;
		gameWon = false;
		player = playerObject.GetComponent<Player>();
		retry = GameObject.FindGameObjectWithTag("Retry");
		gameMenu = GameObject.FindGameObjectWithTag("Menu");
		retry.SetActive(false);
		gameMenu.SetActive(false);
		InitializeLaneList();
		StartCoroutine(Timer());
	}

	void Update()
	{
		HandlePitchChange();
		if (!handledPlayer)
		{
			HandlePlayerWin();
			HandlePlayerDied();
		}
	}

	private void HandlePlayerWin() 
	{
		if (playerObject.transform.position.x == (end_column - 0.5f))
		{
			GameState.gamePlaying = false;
			handledPlayer = true;
			gamePlaying = false;
			StartCoroutine(PlayerWonCoroutine());
		}
	}

	private void HandlePitchChange()
	{
		if (player.playerLives == 1) {
			gameMusic.pitch = 1.25f;
		} else {
			gameMusic.pitch = 1;
		} 
	}

	private void HandlePlayerDied() 
	{
		if (Player.isDead) {
			GameState.gamePlaying = false;
			SelectedPlayer.died = true;
			handledPlayer = true;
			gamePlaying = false;
			StartCoroutine(PlayerDiedCoroutine());
		}
	}

	IEnumerator PlayerWonCoroutine()
	{
		gameOver.GetComponent<Text>().text = "You Win!";
		GameObject gameoverText = Instantiate(gameOver, canvas.transform);
		PlayClip(playerWon);
		yield return new WaitForSeconds(playerWon.length);
		if (SceneManager.GetActiveScene().buildIndex == 0)
			SceneManager.LoadScene(2);
		else 
		{
			retry.SetActive(true);
			gameMenu.SetActive(true);
		}
	}

	IEnumerator PlayerDiedCoroutine() 
	{
		gameOver.GetComponent<Text>().text = "You Lost!";
		GameObject gameoverText = Instantiate(gameOver, canvas.transform);
		GameObject explosion = Instantiate(playerExplosion, playerObject.transform.position, Quaternion.identity);
		Destroy(playerObject);
		ParticleSystem ps = explosion.GetComponent<ParticleSystem>();
		PlayClip(playerLost);
		float explosionDuration = playerLost.length;
		var main = ps.main;
		main.duration = explosionDuration;
		ps.Play();
		yield return new WaitForSeconds(explosionDuration);
		if (SceneManager.GetActiveScene().buildIndex == 0)
			SceneManager.LoadScene(2);
		else 
		{
			retry.SetActive(true);
			gameMenu.SetActive(true);
		}
	}

	public static void PlayClip(AudioClip clip) {
		m_audio_source.PlayOneShot(clip);
	}

	private void InitializeLaneList() {
		laneNums = new List<int>();
		for (int i = 1; i <= GameState.numLanes; i++)
			laneNums.Add(i);
	}

	public IEnumerator Timer() 
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
		gamePlaying = true;
		gameMusic.Play();
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
