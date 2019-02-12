using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEditor;
using UnityEngine.UI;
using UnityEngine.Windows.Speech;
using TMPro;

public class Trivia : Event
{
    bool GotCorrect;
    static int correct = 0;
    static int failed = 0;
    
    // [SerializeField]
    // private Text m_Hypotheses;

    // [SerializeField]
    // private Text m_Recognitions;
    private TextMeshProUGUI TrivaText;
    private TextMeshProUGUI answer;
    

    public Text Question;
    private string ques = "";
    private string ans = "";

    private DictationRecognizer m_DictationRecognizer;

    protected override void Initialize(){
        TrivaText = GameObject.FindWithTag("trivia").GetComponent<TextMeshProUGUI>();
        TrivaText.text = "";
        answer = GameObject.FindWithTag("answer").GetComponent<TextMeshProUGUI>();
        answer.text = "";
        SetUp();
    }


    protected override IEnumerator MakeTextBlink(){
        yield return null;
    }
    protected override void SetUpEvent(){
        //Debug.Log("list length: " + TriviaList.size);

        m_DictationRecognizer = new DictationRecognizer();

        m_DictationRecognizer.DictationResult += (text, confidence) =>
        {
            //Debug.LogFormat("Dictatiaon result: {0}", text);
            answer.text += text;
            if (answer.text == ans){
                Debug.Log("Answer Corect, Total Corect: " + correct);
                timerPaused = true;
                m_player.MovePlayer();
                TrivaText.text = "Correct!";
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

        TriviaList.getQuestion(ref ques, ref ans);
        Debug.Log("Question: " + ques);
        Debug.Log("Answer: " + ans);
        TrivaText.text = ques;
    }

    private float endDisplayTime = 1.1f;
    
    IEnumerator Reset(){
        yield return new WaitForSeconds(endDisplayTime);
        TrivaText.text = "";
        answer.text = "";
        yield return StartCoroutine("Delay");
        eventCorrect = true;
    }

    protected override IEnumerator HandleIncorrect(){
        StopRecognizer();
        SelectedPlayer.current_speech_fail++;
        answer.text = "";
        TrivaText.text = "";
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
