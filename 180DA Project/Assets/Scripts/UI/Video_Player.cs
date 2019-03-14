using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using System.Linq;
using UnityEngine.UI;

public class Video_Player : MonoBehaviour {

	VideoPlayer videoPlayer;
	private const string gestureVideoPath = "UI/GestureVideos/";
	private const string wrong1 = "wrong_1";
	private const string wrong2 = "wrong_2";
	private const string correct = "correct";
	private string[] tposeClipNames = new string[] {wrong1, wrong2, correct};
	private Text gestureText;
	private bool playVideos;
	private float videoBuffer = 0.3f;

	// change later
	private Dictionary<string, string[]> gestureClipStrings = new Dictionary<string, string[]>() {
		{GestureGame.tpose, new string[] {correct, wrong1, wrong2}}, 
		{GestureGame.fieldGoal, new string[]{correct, wrong1, wrong2}},
		{GestureGame.rightHandDab, new string[]{correct, wrong1, wrong2}},
		{GestureGame.rightHandWave, new string[]{correct, wrong1, wrong2}},
		{GestureGame.leftHandWave, new string[]{correct, wrong1, wrong2}},
		{GestureGame.rightHandRaise, new string[]{correct, wrong1}},
		{GestureGame.leftHandRaise, new string[]{correct, wrong1}}
	};
	
	Dictionary<string, List<VideoClip>> clipDictionary; // = new Dictionary<string, VideoClip[]>
	private List<string> gestures;
	void Awake () {
		playVideos = false;
		videoPlayer = GetComponent<VideoPlayer>();
		clipDictionary = new Dictionary<string, List<VideoClip>>();
		Debug.Log(gestureClipStrings);
		gestures = gestureClipStrings.Keys.ToList();
		gestureText = GameObject.Find("GestureVideo").GetComponent<Text>();
		SetUpClips();
	}

	public void PlayClips(string gesture)
	{
		playVideos = true;
		videoPlayer.Stop();
		StopAllCoroutines();
		StartCoroutine(PlayClipsRoutine(gesture));
	}

	private IEnumerator PlayClipsRoutine(string gesture)
	{
		List<VideoClip> clips = clipDictionary[gesture];
		int length = clips.Count;
		int i = 0;
		while(playVideos)
		{
			i = i % length;
			VideoClip clip = clips[i];
			videoPlayer.clip = clip;
			videoPlayer.Play();
			yield return new WaitForSeconds(videoBuffer);
			ChangeText(clip.name);
			yield return new WaitForSeconds((float)clip.length);
			i++;
		}
	}

	private void ChangeText(string clipName)
	{
		if (clipName.StartsWith("wrong"))
			{
				gestureText.text = "NOT THIS";
				gestureText.color = Color.red;
			}
			else
			{
				gestureText.text = "NOT THIS";
				gestureText.color = Color.green;
			}
	}

	public void StopClips()
	{
		playVideos = false;
	}

	private void SetUpClips()
	{
		foreach (string gesture in gestures)
		{
			Debug.Log("setting up " + gesture);
			List<VideoClip> clips = new List<VideoClip>();
			foreach (string clipString in gestureClipStrings[gesture])
			{
				string clipPath = gestureVideoPath + gesture + '/' + clipString;
				Debug.Log(clipPath);
				clips.Add(Resources.Load(clipPath) as VideoClip);
			}
			clipDictionary.Add(gesture, clips);
		}
	}
}
