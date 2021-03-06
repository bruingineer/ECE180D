﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEditor;
using UnityEngine.UI;
using UnityEngine.Windows.Speech;
using TMPro;
public class Trivia : Speech {
	
	private TextMeshProUGUI triviaText;
	private TextMeshProUGUI answer;
	private Text Question;

	private string ques = "";
	private string ans = "";

	protected override void Awake() {
        triviaText = GameObject.FindWithTag("trivia").GetComponent<TextMeshProUGUI>();
        triviaText.text = "";
        answer = GameObject.FindWithTag("answer").GetComponent<TextMeshProUGUI>();
        answer.text = "";
		// StartRecognizer();
		base.Awake();
    }

	// ask jose if it can be called multiple times
	protected override void handleSpeechTask(string text){
		// answer.text = "";
		// answer.text = text;
		if (text == ans)
			answer.text = ans;	
			HandleCorrectEvent();
	}

	protected override void HandleCorrectEvent()
	{
		StopRecognizer();
		timerStopped = true;
		HandleCorrectAction();
		triviaText.text = "Correct!";
		Reset();
		SelectedPlayer.current_trivia_pass++; // put in handle correct action
        SelectedPlayer.current_trivia_timer_avg += Event.curTime;
        SelectedPlayer.new_score = true;
        Debug.Log("curTime: " + Event.curTime);
	}

	protected override void SetUpEvent(string phrase = null){
		Debug.Log(phrase);
		SpeechList.getQuestionNumber(ref ques, ref ans, phrase != null ? Int32.Parse(phrase) : -1);
		Debug.Log("Question: " + ques);
		Debug.Log("Answer: " + ans);
        Debug.Log(SelectedPlayer.current_difficulty);
		triviaText.text = ques;
		StartRecognizer();
		if (m_DictationRecognizer == null){
			recognizer_started = false;
			StartRecognizer();
		}
		m_DictationRecognizer.Start();
	}

	protected override IEnumerator ResetCorrect(){
	    yield return base.ResetCorrect();
        triviaText.text = "";
        answer.text = "";
	}

	protected override void HandleIncorrectEvent(){
		StopRecognizer();
        timerStopped = true;
		Powerup.powerup_count = 0;
        SelectedPlayer.current_trivia_fail++;
        SelectedPlayer.new_score = true;
        answer.text = "";
        triviaText.text = "";
    }

	protected override IEnumerator MakeTextBlink(){
		return null;
	}	
}

public class TriviaMiniGame : Trivia {

	// number of current gestures correct increments when it is a minigame
	protected override void Awake()
	{
		base.Awake();
		GameState_Event_Minigame.numCorrect = 5;
	}
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