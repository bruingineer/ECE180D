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
    
    
    private int triviaORScrabmle;
    public static bool recognizer_started = false;

    protected KeywordRecognizer m_DictationRecognizer;

    protected abstract void handleSpeechTask(string text);

    static int count = 0;

    private void DictationRecognizer_DictationResult(PhraseRecognizedEventArgs args){
        handleSpeechTask(args.text);
    }
    protected void StartRecognizer(){
        if (!recognizer_started){
            recognizer_started = true;
            Debug.Log("starting rec");
            count++;
            m_DictationRecognizer = new KeywordRecognizer(SpeechList.WordList_);
            m_DictationRecognizer.OnPhraseRecognized += DictationRecognizer_DictationResult;
            GotCorrect = false;
        }
    }


    protected void StopRecognizer(){        
        if (m_DictationRecognizer == null){
            Debug.Log("now null");
        }
        else{
            Debug.Log(m_DictationRecognizer.IsRunning);
        }
        if (m_DictationRecognizer != null /*&& (m_DictationRecognizer.Status == UnityEngine.Windows.Speech.SpeechSystemStatus.Running)*/)
        {
            Debug.Log("Closing Dictation Recognizer");
            m_DictationRecognizer.Stop();
            m_DictationRecognizer.OnPhraseRecognized -= DictationRecognizer_DictationResult;
            m_DictationRecognizer.Dispose();
            
        }
        recognizer_started = false;
        return;
    }

    protected override void Reset()
	{	
		StartCoroutine(ResetCorrect());
	}

    protected override IEnumerator ResetCorrect()
    {
        yield return new WaitForSeconds(endDisplayTime);
        eventCorrect = true;
    }

    private void OnApplicationQuit()
    {
        StopRecognizer();
    }
}
