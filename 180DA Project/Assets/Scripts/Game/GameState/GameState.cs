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
	public static AudioSource gameMusic;
	protected AudioClip gameLostMusic;
	protected AudioClip gameWonMusic;
	private const string SoundsPath = "Sounds/Game Music/";
	public static string gameMode;

	// Objects
	protected Canvas canvas;
	public Text result;
	public Text countdown;
	static Button retry, menu;
	public static bool gamePlaying;
	private static GameState_Base instance;

    //MQTT Client to update db when training completed
    MQTTHelper training_client;

    void Awake () {
        training_client = new MQTTHelper("database");
		Time.timeScale = 1;
		instance = this;
		numLanes = 10;
		end_row = 14;
		gameLostMusic = Resources.Load<AudioClip>(SoundsPath + "Game_Lost");
		gameWonMusic = Resources.Load<AudioClip>(SoundsPath + "Game_Won");
		gamePlaying = false;
		canvas = GameObject.Find("Canvas").GetComponent<Canvas>();
		gameMusic = GetComponent<AudioSource>();
		retry = Instantiate((Resources.Load("Prefabs/WorldSpace/Retry") as GameObject), canvas.transform).GetComponent<Button>();
		retry.onClick.AddListener(RetryLevel);
		menu = Instantiate((Resources.Load("Prefabs/WorldSpace/Menu") as GameObject), canvas.transform).GetComponent<Button>();
		menu.onClick.AddListener(LoadGameMenu);
		DisableButtons();
		SetUp();
	}

	private IEnumerator StartGameCoroutine()
	{
		yield return StartCoroutine(GameTimer());
		gameMusic.Play();
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

	public static void StartGame()
	{
		instance.StartCoroutine(instance.StartGameCoroutine());
	}

	public static void PlayClip(AudioClip clip) {
		gameMusic.PlayOneShot(clip);
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
		HandleCountdown();
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
		SceneManager.LoadScene("Game Menu");
	}

	protected void LoadStatsMenu()
	{
		SceneManager.LoadScene("End Game");
	}

	// create function to set up button
	public static void SetUpButtons()
	{
		retry.gameObject.SetActive(true);
		menu.gameObject.SetActive(true);
	}

	protected virtual void HandleCountdown()
	{
		Destroy(countdown);
	}

	public static void DisableButtons()
	{
		retry.gameObject.SetActive(false);
		menu.gameObject.SetActive(false);
	}

	protected IEnumerator HandlePostGame(float length)
	{
		yield return new WaitForSeconds(length);
		HandlePostGameScene();
	}

	protected void GameWon()
	{
		gamePlaying = false;
		result.text = "You win!";
		PlayClip(gameWonMusic);
        training_client.CheckIfTrainingComplete(gameMode);
		StartCoroutine(HandlePostGame(gameWonMusic.length));
	}

	protected abstract void SetUp();
	protected abstract void SetUp_Events_Obstacles();
	protected abstract void HandlePostGameScene();
	protected abstract void HandleWin();
	protected virtual void HandleLose() {}

}

public abstract class GameState_with_Player : GameState_Base {
	
	
	protected GameObject player;

	protected override void SetUp()
	{
		InitializeLaneList();
		SelectedPlayer.resetGameStats();
		player = (Resources.Load("Prefabs/Player/Player") as GameObject);
		player = Instantiate(player, new Vector3(numLanes / 2 + 0.5f, 0.5f), Quaternion.identity);
	}

	private void InitializeLaneList() {
		laneNums = new List<int>();
		for (int i = 0; i < numLanes; i++)
			laneNums.Add(i);
	}


	protected override void HandleWin() 
	{
		if (player.transform.position.y == (end_row - 0.5f))
			GameWon();
	}

}

public abstract class GameState_with_Lives : GameState_with_Player {
	protected Player_Main player_Main;
	public GameObject playerExplosion;
	public List<GameObject> playerIcons;

	protected override void SetUp()
	{
		base.SetUp();
		player_Main = player.AddComponent<Player_Main>();
		StartGame();
	}

	public void RemoveLife(int curLives)
	{
		Debug.Log(curLives);
		playerIcons[curLives - 1].SetActive(false);
		ChangePitch(curLives);
	}

	public void ChangePitch(int playerLives)
	{
		gameMusic.pitch = playerLives > 1 ? 1 : 1.25f;
	}

	protected override void HandleLose() 
	{
		if (player_Main.isDead) {
			gamePlaying = false;
			SelectedPlayer.died = true;
			result.text = "Game Over!";
			// add for handle lose for multiplayer
			ParticleSystem explosion = Instantiate(playerExplosion, player.transform.position, Quaternion.identity).GetComponent<ParticleSystem>();
			Destroy(player.gameObject);
			PlayClip(gameLostMusic);
			float explosionDuration = gameLostMusic.length;
			var main = explosion.main;
			main.duration = explosionDuration;
			explosion.Play();
			StartCoroutine(HandlePostGame(explosionDuration));
		}
	}

}


public abstract class GameState_Event_Minigame : GameState_Base {
	private int numCorrect = 5;
	public static int curCorrect;

	protected override void SetUp()
	{
		curCorrect = 0;
		StartGame();
	}

	protected override void HandleWin()
	{
		if(curCorrect == numCorrect)
			GameWon();
	}

	protected override void HandlePostGameScene()
	{
		SetUpButtons();
	}
}