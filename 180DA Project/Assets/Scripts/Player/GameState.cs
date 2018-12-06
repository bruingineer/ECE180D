using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameState : MonoBehaviour {

	public static int numLanes;
	public static int middle_lane;
	public GameObject player;
	public Canvas canvas;
	public static int end_column;
	private static AudioSource m_audio_source;
	public GameObject gameOver;
	public GameObject countdown;
	public AudioSource gameMusic;
	public static bool gameStarted;
	public bool handledPlayerDied;
	public GameObject playerExplosion;
	public AudioClip playerLost;
	
	void Awake () {
		numLanes = 10;
		middle_lane = numLanes/2;
		end_column = 20;
		m_audio_source = GetComponent<AudioSource>();
		gameStarted = false;
		handledPlayerDied = false;
		StartCoroutine(Timer());
	}

	void Update()
	{
		// if (Player.playerLives == 1) {
		// 	gameMusic.pitch = 1.25f;
		// } else {
		// 	gameMusic.pitch = 1;
		// } 
		if (Player.isDead && !handledPlayerDied) {
			StartCoroutine(HandlePlayerDied());
		}
	}

	IEnumerator HandlePlayerDied () 
	{
		handledPlayerDied = true;
		GameObject gameoverText = Instantiate(gameOver, canvas.transform);
		GameObject explosion = Instantiate(playerExplosion, player.transform.position, Quaternion.identity);
		Destroy(player);
		ParticleSystem ps = explosion.GetComponent<ParticleSystem>();
		GameState.PlayClip(playerLost);
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
		gameStarted = true;
		gameMusic.Play();
	}
}
