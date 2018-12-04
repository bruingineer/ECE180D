using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameState : MonoBehaviour {

	public static int numLanes;
	public static int middle_lane;

	public static int end_column;
	private static AudioSource m_audio_source;
	
	void Awake () {
		// change to make it dynamic
		numLanes = 10;
		middle_lane = numLanes/2;
		end_column = 20;
		m_audio_source = GetComponent<AudioSource>();
	}

	public static void PlayClip(AudioClip clip) {
		m_audio_source.PlayOneShot(clip);
	}

}
