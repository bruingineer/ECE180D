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
		Answer.text = "";
		Answer.text = text;
		Debug.Log(text);
		if (Answer.text == WDisplay.word_str)
		{
			timerPaused = true;
			m_player.MovePlayer();
			WDisplay.WordText.text = "Correct!";
			StopRecognizer();
			SelectedPlayer.current_unscramble_pass++;
			StartCoroutine("Reset");
		}
	}

	protected override void Initialize(){
        WDisplay = new WordDisplay();
		WDisplay.WordText = GameObject.FindWithTag("trivia").GetComponent<TextMeshProUGUI>();
        WDisplay.WordText.text = "";
        Answer = GameObject.FindWithTag("answer").GetComponent<TextMeshProUGUI>();
        Answer.text = "";
        SetUp();
    }

	protected override void SetUpEvent(){       
		Debug.Log("Starting Scramble");
		string w = "";
		w = SpeechList.getWord();
		WDisplay.SetWordDisplay(w);
		WDisplay.SetFirstLetterClue();
		Debug.Log(w);
		Debug.Log(WDisplay.WordText.text);
		StartRecognizer();
		m_DictationRecognizer.Start();
    }

	protected override IEnumerator Reset(){
		yield return new WaitForSeconds(endDisplayTime);
        WDisplay.WordText.text = "";
        Answer.text = "";
        yield return StartCoroutine("Delay");
        eventCorrect = true;
	}

	protected override IEnumerator MakeTextBlink(){
            while(!timerPaused){
            //Debug.Log("making word blink");
            WDisplay.MakeWordBlink();
			yield return new WaitForSeconds(repeatRate/1.5f);
        }
    }

	protected override IEnumerator HandleIncorrect(){
        timerPaused = true;
        StopRecognizer();
        SelectedPlayer.current_unscramble_fail++;
        WDisplay.WordText.text = "";
        Answer.text = "";
        yield return StartCoroutine("Delay");
    }

}
