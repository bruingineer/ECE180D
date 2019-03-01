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
			HandleCorrectEvent();
	}

	protected override void HandleCorrectEvent()
	{
		timerStopped = true;
		HandleCorrectAction();
		StopRecognizer();
		WDisplay.WordText.text = "Correct!";
		SelectedPlayer.current_unscramble_pass++;
        SelectedPlayer.current_unscramble_timer_avg += Event.curTime;
        Debug.Log("curTime: " + Event.curTime);
        Reset();
	}

	protected override void Event_Initializer(){
        WDisplay = new WordDisplay();
		WDisplay.WordText = GameObject.FindWithTag("trivia").GetComponent<TextMeshProUGUI>();
        WDisplay.WordText.text = "";
        Answer = GameObject.FindWithTag("answer").GetComponent<TextMeshProUGUI>();
        Answer.text = "";
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

	protected override IEnumerator Reset_Speech_Correct()
	{
		yield return base.Reset_Speech_Correct();
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
        timerStopped = true;
        StopRecognizer();
        SelectedPlayer.current_unscramble_fail++;
        WDisplay.WordText.text = "";
        Answer.text = "";
    }
}

public class WordScrambleMiniGame : WordScramble {

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
