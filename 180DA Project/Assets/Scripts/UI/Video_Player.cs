using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using System.Linq;

public class Video_Player : MonoBehaviour {

	VideoPlayer videoPlayer;
	private const string gestureVideoPath = "UI/Gestures/";
	private const string tpose = "TPose/";
	private const string wrong1 = "wrong_1";
	private const string wrong2 = "wrong_2";
	private const string correct = "correct";
	private string[] tposeClipNames = new string[] {wrong1, wrong2, correct};

	private Dictionary<string, string[]> gestureClipStrings = new Dictionary<string, string[]>() {
		{"tpose", new string[] {wrong1, wrong2, correct}}, 
		{"fieldgoal", new string[]{}},
		{"dab", new string[]{}},
		{"righthandwave", new string[]{}},
		{"lefthandraise", new string[]{}},
		{"righthandraise", new string[]{}},
	};

	private Dictionary<string, string> gesturePaths = new Dictionary<string, string>() {
		{"tpose", tpose}, 
		{"fieldgoal", ""},
		{"dab", ""},
		{"righthandwave", ""},
		{"lefthandraise", ""},
		{"righthandraise", ""},
	};
	
	Dictionary<string, List<VideoClip>> clipDictionary; // = new Dictionary<string, VideoClip[]>
	private List<string> gestures;
	void Awake () {
		videoPlayer = GetComponent<VideoPlayer>();
		clipDictionary = new Dictionary<string, List<VideoClip>>();
		Debug.Log(gestureClipStrings);
		gestures = gestureClipStrings.Keys.ToList();
		SetUpClips();
	}

	public void PlayClips(string gesture)
	{
		
	}

	private void SetUpClips()
	{
		foreach (string gesture in gestures)
		{
			Debug.Log("setting up " + gesture);
			List<VideoClip> clips = new List<VideoClip>();
			foreach (string clipString in gestureClipStrings[gesture])
			{
				clips.Add(Resources.Load(gestureVideoPath + gesturePaths[gesture] + clipString) as VideoClip);
			}
			clipDictionary.Add(gesture, clips);
		}
	}
	
	
}
