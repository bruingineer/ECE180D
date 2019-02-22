using System.Collections;
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

	protected override void Event_Initializer(){
        triviaText = GameObject.FindWithTag("trivia").GetComponent<TextMeshProUGUI>();
        triviaText.text = "";
        answer = GameObject.FindWithTag("answer").GetComponent<TextMeshProUGUI>();
        answer.text = "";
    }

	// ask jose if it can be called multiple times
	protected override void handleSpeechTask(string text){
		answer.text = "";
		answer.text = text;
		if (answer.text == ans)
			HandleCorrectEvent();
	}

	protected override void HandleCorrectEvent()
	{
		timerStopped = true;
		HandleCorrectAction();
		triviaText.text = "Correct!";
		StopRecognizer();
		Reset();
		SelectedPlayer.current_trivia_pass++; //need to change
	}

	protected override void SetUpEvent(){
		Debug.Log("Starting trivia");
		SpeechList.getQuestion(ref ques, ref ans);
		Debug.Log("Question: " + ques);
		Debug.Log("Answer: " + ans);
		triviaText.text = ques;
		Debug.Log(triviaText.text);
		StartRecognizer();
		m_DictationRecognizer.Start();
	}

	protected override IEnumerator Reset_Speech_Correct(){
	    yield return base.Reset_Speech_Correct();
        triviaText.text = "";
        answer.text = "";
	}

	protected override void HandleIncorrectEvent(){
        timerStopped = true;
        StopRecognizer();
        SelectedPlayer.current_trivia_fail++;
        answer.text = "";
        triviaText.text = "";
    }

	protected override IEnumerator MakeTextBlink(){
		return null;
	}	
}

public class TriviaMiniGame : Trivia {

	// number of current gestures correct increments when it is a minigame
	protected override void HandleCorrectAction()
	{
		GameState_Event_Minigame.curCorrect++;
	}

	// number of correct gestures resets to 0 when incorrect
	protected override void HandleIncorrectEvent()
	{
		GameState_Event_Minigame.curCorrect = 0;
	}
}