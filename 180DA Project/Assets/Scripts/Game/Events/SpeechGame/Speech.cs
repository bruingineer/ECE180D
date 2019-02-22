using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEditor;
using UnityEngine.UI;
using UnityEngine.Windows.Speech;
using TMPro;

public abstract class Speech : Event
{
    WordDisplay WDisplay;
    bool GotCorrect;
    static int correct = 0;
    static int failed = 0;
    
    // [SerializeField]
    // private Text m_Hypotheses;

    // [SerializeField]
    // private Text m_Recognitions;
    //protected TextMeshProUGUI triviaText;
    //protected TextMeshProUGUI answer;
    
    private int triviaORScrabmle;
    //protected Text Question;
    //private string ques = "";
    //private string ans = "";
    protected static bool recognizer_started = false;

    protected DictationRecognizer m_DictationRecognizer;

    // protected override void Initialize(){
    //     WDisplay = new WordDisplay();
    //     triviaText = GameObject.FindWithTag("trivia").GetComponent<TextMeshProUGUI>();
    //     WDisplay.WordText = GameObject.FindWithTag("trivia").GetComponent<TextMeshProUGUI>();
    //     triviaText.text = "";
    //     answer = GameObject.FindWithTag("answer").GetComponent<TextMeshProUGUI>();
    //     answer.text = "";
    //     // Msg = GameObject.FindWithTag("msg").GetComponent<TextMeshProUGUI>();
    //     // Msg.text = "";
    //     SetUp();
    // }

    // protected override IEnumerator MakeTextBlink(){
    //         while(triviaORScrabmle == 1 && !timerPaused){
    //         //Debug.Log("making word blink");
    //         WDisplay.MakeWordBlink();
	// 		yield return new WaitForSeconds(repeatRate/1.5f);
    //     }
    // }


    protected abstract void handleSpeechTask(string text);


    protected void StartRecognizer(){
        if (!recognizer_started){
            m_DictationRecognizer = new DictationRecognizer();

            m_DictationRecognizer.DictationResult += (text, confidence) =>
            {
                handleSpeechTask(text);
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
        }
    }

    protected float endDisplayTime = 1.1f;

    protected void StopRecognizer(){
        //Debug.Log("app quit");
        if (m_DictationRecognizer != null && (m_DictationRecognizer.Status == UnityEngine.Windows.Speech.SpeechSystemStatus.Running))
        {
            //Debug.Log("Closing Dictation Recognizer");
            m_DictationRecognizer.Dispose();
            m_DictationRecognizer.Stop();
        }
    }

    protected override void Reset()
	{	
		StartCoroutine(Reset_Speech_Correct());
	}

    protected virtual IEnumerator Reset_Speech_Correct()
    {
        yield return new WaitForSeconds(endDisplayTime);
        eventCorrect = true;
    }

    private void OnApplicationQuit()
    {
        StopRecognizer();
    }
}
