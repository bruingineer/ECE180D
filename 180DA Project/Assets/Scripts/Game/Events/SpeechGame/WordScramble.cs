using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEditor;
using UnityEngine.UI;
using UnityEngine.Windows.Speech;
using TMPro;

public class WordScramble : Speech {

	WordDisplay WDisplay;
	private TextMeshProUGUI Answer;
	
	private static int failed = 0;

	protected override void handleSpeechTask(string text){
		// Answer.text = "";
		// Answer.text = text;
		//Debug.Log(text);
		if (text == WDisplay.word_str)
			Answer.text = text;
			HandleCorrectEvent();
	}

	protected override void HandleCorrectEvent()
	{
		timerStopped = true;
		Debug.Log("calling stop recognizer");
		StopRecognizer();
		HandleCorrectAction();
		//WDisplay.WordText.fontSize = 40;
		WDisplay.WordText.text = "Correct!";
		WDisplay.WordText.characterSpacing = default_spacing; 
		SelectedPlayer.current_unscramble_pass++;
        SelectedPlayer.current_unscramble_timer_avg += Event.curTime;
        SelectedPlayer.new_score = true;
        //Debug.Log("curTime: " + Event.curTime);
        Reset();
	}

	protected override void Awake() {
        WDisplay = new WordDisplay();
		WDisplay.WordText = GameObject.FindWithTag("word").GetComponent<TextMeshProUGUI>();
        WDisplay.WordText.text = "";
        Answer = GameObject.FindWithTag("answer").GetComponent<TextMeshProUGUI>();
        Answer.text = "";
		base.Awake();
		// StartRecognizer();
    }

	private float default_spacing;
	protected override void SetUpEvent(string phrase = null){  
		//WDisplay.WordText.fontSize = 60;   
		default_spacing = WDisplay.WordText.characterSpacing;
		WDisplay.WordText.characterSpacing = 5f;
		Debug.Log("Starting Scramble");
		string w = "";
		
		w = SpeechList.getWord(phrase != null ? Int32.Parse(phrase) : -1);
		
		WDisplay.SetWordDisplay(w);
		WDisplay.SetFirstLetterClue();
		Debug.Log(w);
		Debug.Log(WDisplay.WordText.text);
		StartRecognizer();
		if (m_DictationRecognizer == null){
			Speech.recognizer_started = false;
			StartRecognizer();
		}
		m_DictationRecognizer.Start();
    }

	protected override IEnumerator ResetCorrect()
	{
		yield return base.ResetCorrect();
		WDisplay.WordText.text = "";
        Answer.text = "";
	}

	protected override IEnumerator MakeTextBlink(){
            while(!timerStopped){
            WDisplay.MakeWordBlink();
			yield return new WaitForSeconds(repeatRate/1.5f);
        }
    }

	// ask jose if he can have one word shared by speech events
	protected override void HandleIncorrectEvent(){
		
		Powerup.powerup_count = 0;
		//WDisplay.WordText.fontSize = 40;
		WDisplay.WordText.characterSpacing = default_spacing;
        timerStopped = true;
		Debug.Log("calling stop recognizer");
        StopRecognizer();
        SelectedPlayer.current_unscramble_fail++;
        SelectedPlayer.new_score = true;
        WDisplay.WordText.text = "";
        Answer.text = "";
    }
}

public class WordScrambleMiniGame : WordScramble {

	protected override void Awake()
	{
		base.Awake();
		GameState_Event_Minigame.numCorrect = 5;
	}

	// number of current gestures correct increments when it is a minigame
	protected override void HandleCorrectAction()
	{
		GameState_Event_Minigame.curCorrect++;
	}

	// number of correct gestures resets to 0 when incorrect
	protected override void HandleIncorrectEvent()
	{
		GameState_Event_Minigame.curCorrect = 0;
		StopRecognizer();
	}
}
