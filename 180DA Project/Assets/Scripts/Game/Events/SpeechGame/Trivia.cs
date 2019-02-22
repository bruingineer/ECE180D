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

	protected override void Initialize(){
        triviaText = GameObject.FindWithTag("trivia").GetComponent<TextMeshProUGUI>();
        triviaText.text = "";
        answer = GameObject.FindWithTag("answer").GetComponent<TextMeshProUGUI>();
        answer.text = "";
        SetUp();
    }

	protected override void handleSpeechTask(string text){
		answer.text = "";
		answer.text = text;
		if (answer.text == ans){
			timerPaused = true;
			m_player.MovePlayer();
			triviaText.text = "Correct!";
			StopRecognizer();
			SelectedPlayer.current_trivia_pass++; //need to change
			StartCoroutine("Reset");
		}
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

	protected override IEnumerator Reset(){
	    yield return new WaitForSeconds(endDisplayTime);
        triviaText.text = "";
        answer.text = "";
        yield return StartCoroutine("Delay");
        eventCorrect = true;
	}

	protected override IEnumerator HandleIncorrect(){
        timerPaused = true;
        StopRecognizer();
        SelectedPlayer.current_trivia_fail++;
        answer.text = "";
        triviaText.text = "";
        yield return StartCoroutine("Delay");
    }

	protected override IEnumerator MakeTextBlink(){
		return null;
	}	
}
