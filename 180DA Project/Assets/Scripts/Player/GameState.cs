using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

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
	
	void Awake () {
		numLanes = 10;
		middle_lane = numLanes/2;
		end_column = 20;
		m_audio_source = GetComponent<AudioSource>();
		gamePlaying = false;
		handledPlayer = false;
		gameWon = false;
		player = playerObject.GetComponent<Player>();
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
		SceneManager.LoadScene(2);
	}

	IEnumerator PlayerDiedCoroutine() 
	{
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
		SceneManager.LoadScene(2);
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
}
