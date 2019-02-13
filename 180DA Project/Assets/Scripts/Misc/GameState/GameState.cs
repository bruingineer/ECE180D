using System.Collections;
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

	void Update()
	{
		if (gamePlaying)
		{
			HandleWin();
			HandleLose();
		}
	}

	protected virtual void SetUp() {}

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

	// add these to a scenemanager static class
	public void RetryLevel()
	{
		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
	}

	// change to just one static function and constants for scenes
	public void LoadGameMenu()
	{
		SceneManager.LoadScene(5);
	}

	protected void LoadStatsMenu()
	{
		SceneManager.LoadScene(2);
	}

	// create function to set up button
	protected void SetUpButtons()
	{
		Button retry, menu;
		retry = Instantiate((Resources.Load("Prefabs/WorldSpace/Retry") as GameObject), canvas.transform).GetComponent<Button>();
		retry.onClick.AddListener(RetryLevel);
		menu = Instantiate((Resources.Load("Prefabs/WorldSpace/Menu") as GameObject), canvas.transform).GetComponent<Button>();
		menu.onClick.AddListener(LoadGameMenu);
	}

	protected IEnumerator HandlePostGame(float length)
	{
		yield return new WaitForSeconds(length);
		HandlePostGameScene();
	}

	protected abstract void HandlePostGameScene();
	protected abstract void HandleWin();
	protected abstract void HandleLose();

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

	public void RemoveLife(int curLives)
	{
		playerLives[curLives - 1].SetActive(false);
		ChangePitch(curLives);
	}

	protected override void HandleWin() 
	{
		if (player.transform.position.y == (end_row - 0.5f))
		{
			gamePlaying = false;
			result.text = "You win!";
			PlayClip(gameWon);
			StartCoroutine(HandlePostGame(gameWon.length));
		}
	}

	protected override void HandleLose() 
	{
		if (player.isDead) {
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
			StartCoroutine(HandlePostGame(explosionDuration));
		}
	}

}

public abstract class GameState_Event_Minigame : GameState_Base {
	private int numCorrect = 5;
	public static int curCorrect = 0;

	protected override void SetUp()
	{
		curCorrect = 0;
	}

	protected override void HandleWin()
	{
		if(curCorrect == numCorrect)
		{
			gamePlaying = false;
			result.text = "You win!";
			PlayClip(gameWon);
			StartCoroutine(HandlePostGame(gameWon.length));
		}
	}

	protected override void HandlePostGameScene()
	{
		SetUpButtons();
	}

	protected override void HandleLose()
	{
		// Handle loss later lol
	}
}