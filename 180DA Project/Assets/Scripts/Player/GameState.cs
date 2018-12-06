using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameState : MonoBehaviour {

	public static int numLanes;
	public static int middle_lane;

	public static int end_column;
	private static AudioSource m_audio_source;
	public static Text gameOver;
	public GameObject countdown;

	public AudioSource gameMusic;
	public static bool gameStarted;
	
	void Awake () {
		numLanes = 10;
		middle_lane = numLanes/2;
		end_column = 20;
		m_audio_source = GetComponent<AudioSource>();
		gameOver = GameObject.Find("GameOver").GetComponent<Text>();
		gameOver.enabled = false;
		gameStarted = false;
		StartCoroutine(Timer());
	}

	void Update()
	{
		if (Player.playerLives == 1) {
			gameMusic.pitch = 1.25f;
		} else {
			gameMusic.pitch = 1;
		} 
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
