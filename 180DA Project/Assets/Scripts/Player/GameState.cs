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

	public AudioSource gameMusic;
	
	void Awake () {
		numLanes = 10;
		middle_lane = numLanes/2;
		end_column = 20;
		m_audio_source = GetComponent<AudioSource>();
		gameOver = GameObject.Find("GameOver").GetComponent<Text>();
		gameOver.enabled = false;
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

	public static IEnumerator Timer(GameObject gameObject) 
	{
		float duration = 3f; // 3 seconds you can change this to
      	//to whatever you want
      	float totalTime = 0;
      	while(totalTime <= duration)
      	{
          totalTime += Time.deltaTime;
          var integer = (int)totalTime; /* choose how to quantize this */
          /* convert integer to string and assign to text */
          yield return null;
      	}
		Destroy(gameObject);
	}

}
