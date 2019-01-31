using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEditor;
using UnityEngine.UI;
using UnityEngine.Windows.Speech;

public class Trivia : MonoBehaviour
{
    bool GotCorrect;
    static int correct = 0;
    static int failed = 0;
    [SerializeField]
    private Text m_Hypotheses;

    [SerializeField]
    private Text m_Recognitions;

    public Text Question;
    private string ques = "";
    private string ans = "";

    private DictationRecognizer m_DictationRecognizer;
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("list length: " + TriviaList.size);

        m_DictationRecognizer = new DictationRecognizer();

        m_DictationRecognizer.DictationResult += (text, confidence) =>
        {
            //Debug.LogFormat("Dictation result: {0}", text);
            m_Recognitions.text += text;
        };

        m_DictationRecognizer.DictationHypothesis += (text) =>
        {
            //Debug.LogFormat("Dictation hypothesis: {0}", text);
            m_Hypotheses.text += text;
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

        GotCorrect = false;
        m_DictationRecognizer.Start();

        TriviaList.getQuestion(ref ques, ref ans);
        Debug.Log("Question: " + ques);
        Debug.Log("Answer: " + ans);
        Question.text = ques;
        
    }

    

    // Update is called once per frame
    void Update()
    {
       if (m_Recognitions.text == ans && !GotCorrect){
            GotCorrect = true;
            correct++;
            Debug.Log("Answer Corect, Total Corect: " + correct);
       } 
    }

    private void OnApplicationQuit()
    {
        //Debug.Log("app quit");
        if (m_DictationRecognizer != null && (m_DictationRecognizer.Status == UnityEngine.Windows.Speech.SpeechSystemStatus.Running))
        {
            //Debug.Log("Closing Dictation Recognizer");
            m_DictationRecognizer.Dispose();
            m_DictationRecognizer.Stop();
        }
    }
}
