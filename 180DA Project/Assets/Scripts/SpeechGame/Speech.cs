﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEditor;
using UnityEngine.UI;
using UnityEngine.Windows.Speech;
using TMPro;

public class Speech : Event
{
    WordDisplay WDisplay;
    bool GotCorrect;
    static int correct = 0;
    static int failed = 0;
    
    // [SerializeField]
    // private Text m_Hypotheses;

    // [SerializeField]
    // private Text m_Recognitions;
    private TextMeshProUGUI triviaText;
    private TextMeshProUGUI answer;
    
    private int triviaORScrabmle;
    public Text Question;
    private string ques = "";
    private string ans = "";

    private DictationRecognizer m_DictationRecognizer;

    protected override void Initialize(){
        WDisplay = new WordDisplay();
        triviaText = GameObject.FindWithTag("trivia").GetComponent<TextMeshProUGUI>();
        WDisplay.WordText = GameObject.FindWithTag("trivia").GetComponent<TextMeshProUGUI>();
        triviaText.text = "";
        answer = GameObject.FindWithTag("answer").GetComponent<TextMeshProUGUI>();
        answer.text = "";
        // Msg = GameObject.FindWithTag("msg").GetComponent<TextMeshProUGUI>();
        // Msg.text = "";
        SetUp();
    }


    protected override IEnumerator MakeTextBlink(){
            while(triviaORScrabmle == 1 && !timerPaused){
            //Debug.Log("making word blink");
            WDisplay.MakeWordBlink();
			yield return new WaitForSeconds(repeatRate/1.5f);
        }
    }
    protected override void SetUpEvent(){
        m_DictationRecognizer = new DictationRecognizer();

        m_DictationRecognizer.DictationResult += (text, confidence) =>
        {
            //Debug.LogFormat("Dictatiaon result: {0}", text);
            answer.text = "";
            answer.text = text;
            Debug.Log(text);
            if (answer.text == ans){
                Debug.Log("Answer Corect, Total Corect: " + correct);
                timerPaused = true;
                m_player.MovePlayer();
                triviaText.text = "Correct!";
                StopRecognizer();
                SelectedPlayer.current_speech_pass++;
                StartCoroutine("Reset");
            }
        };

        m_DictationRecognizer.DictationHypothesis += (text) =>
        {
            //Debug.LogFormat("Dictation hypothesis: {0}", text);
            //m_Hypotheses.text += text;
        };

        m_DictationRecognizer.DictationComplete += (completionCause) =>
        {
            if (completionCause != DictationCompletionCause.Complete)
                Debug.LogErrorFormat("Dictation completed unsuccessfully: {0}.", completionCause);
        };

        m_DictationRecognizer.DictationError += (error, hresult) =>
        {
            Debug.LogErrorFormat("Dictation error: {0}; HResult = {1}.", error, hresult);
        };

        m_DictationRecognizer.InitialSilenceTimeoutSeconds = 14f;
        GotCorrect = false;
        m_DictationRecognizer.Start();
        

        triviaORScrabmle = UnityEngine.Random.Range(0,2);
        if (triviaORScrabmle == 0)
        {
            Debug.Log("Starting trivia");
            SpeechList.getQuestion(ref ques, ref ans);
            Debug.Log("Question: " + ques);
            Debug.Log("Answer: " + ans);
            triviaText.text = ques;
            Debug.Log(triviaText.text);
        }
        else{
            Debug.Log("Starting Scramble");
            ans = SpeechList.getWord();
            WDisplay.SetWordDisplay(ans);
            Debug.Log(ans);
            Debug.Log(WDisplay.WordText.text);
        }
        // if (m_DictationRecognizer.Status != UnityEngine.Windows.Speech.SpeechSystemStatus.Running)
        //     Debug.Log("not running");
    }

    private float endDisplayTime = 1.1f;
    
    IEnumerator Reset(){
        yield return new WaitForSeconds(endDisplayTime);
        triviaText.text = "";
        answer.text = "";
        yield return StartCoroutine("Delay");
        eventCorrect = true;
    }

    protected override IEnumerator HandleIncorrect(){
        timerPaused = true;
        StopRecognizer();
        SelectedPlayer.current_speech_fail++;
        answer.text = "";
        triviaText.text = "";
        yield return StartCoroutine("Delay");
    }

    private void StopRecognizer(){
        //Debug.Log("app quit");
        if (m_DictationRecognizer != null && (m_DictationRecognizer.Status == UnityEngine.Windows.Speech.SpeechSystemStatus.Running))
        {
            //Debug.Log("Closing Dictation Recognizer");
            m_DictationRecognizer.Dispose();
            m_DictationRecognizer.Stop();
        }
    }

    private void OnApplicationQuit()
    {
        StopRecognizer();
    }
}
