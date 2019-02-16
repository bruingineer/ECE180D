using System.Collections;
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
    static int correct = 0;
    private TextMeshProUGUI triviaText;
    private TextMeshProUGUI answer;
    private int triviaORScrabmle;
    private string ques = "";
    private string ans = "";

    private DictationRecognizer m_DictationRecognizer;

    protected override void Event_Initializer(){
        triviaText = GameObject.FindWithTag("trivia").GetComponent<TextMeshProUGUI>();
        triviaText.text = "";
        answer = GameObject.FindWithTag("answer").GetComponent<TextMeshProUGUI>();
        answer.text = "";
    }


    protected override IEnumerator MakeTextBlink(){
            while(triviaORScrabmle == 1 && !timerStopped){
            WDisplay.MakeWordBlink();
			yield return new WaitForSeconds(repeatRate/1.5f);
        }
    }

    protected override IEnumerator HandleCorrectEvent()
    {
        Debug.Log("Answer Correct, Total Correct: " + correct);
                timerStopped = true;
                HandleCorrectAction();
                triviaText.text = "Correct!";
                StopRecognizer();
                SelectedPlayer.current_speech_pass++;
                yield return Reset();
    }

    protected override void HandleCorrectAction()
    {
        m_player.MovePlayer();
    }
    protected override void SetUpEvent(){
        m_DictationRecognizer = new DictationRecognizer();
        m_DictationRecognizer.DictationResult += (text, confidence) =>
        {
            answer.text = "";
            answer.text = text;
            if (answer.text == ans){
                HandleCorrectEvent();
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
    }

    private float endDisplayTime = 1.1f;
    
    IEnumerator Reset(){
        yield return new WaitForSeconds(endDisplayTime);
        yield return Delay();
        eventCorrect = true;
    }

    protected override IEnumerator Delay()
    {
        answer.text = "";
        triviaText.text = "";
        yield return base.Delay();
    }

    protected override IEnumerator HandleIncorrectEvent(){
        timerStopped = true;
        StopRecognizer();
        SelectedPlayer.current_speech_fail++;
        yield return Delay();
    }

    private void StopRecognizer(){
        if (m_DictationRecognizer != null && (m_DictationRecognizer.Status == UnityEngine.Windows.Speech.SpeechSystemStatus.Running))
        {
            m_DictationRecognizer.Dispose();
            m_DictationRecognizer.Stop();
        }
    }

    private void OnApplicationQuit()
    {
        StopRecognizer();
    }
}
