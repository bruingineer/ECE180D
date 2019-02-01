using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows.Speech;
using UnityEngine.UI;
using TMPro;

public class SpeechRecognitionEngine2 : Event
{
    private WordDisplay WordDisplay;
    public TextMeshProUGUI Msg;
    public ConfidenceLevel confidence = ConfidenceLevel.Medium;
    protected static PhraseRecognizer recognizer;
    public string[] keywords = {""};
    private float endDisplayTime = 1.1f;

    private void SetDisplay(){
        //choose a random word
        System.Random rnd = new System.Random();
        keywords[0] = WordList.WordList_[rnd.Next(WordList.WordList_.Length)];
        WordDisplay.word_str = keywords[0];
        Instantiate(WordDisplay);
        //need to create a TextMeshProUGUI and tag it
        Msg = GameObject.FindGameObjectWithTag("msg").GetComponent<TextMeshProUGUI>();
    }

    protected override void SetUp()
    {
        SetDisplay();
        if (keywords != null){
            recognizer = new KeywordRecognizer(keywords, confidence);
            recognizer.OnPhraseRecognized += Recognizer_OnPhraseRecognized;
            recognizer.Start();
        }
    }


    //TODO: Implement
    // protected override IEnumerator MakeTextBlink(){

    // } 

    void Update(){
        if (Player.isDead)
            Destroy(gameObject);
    }

    private void Recognizer_OnPhraseRecognized(PhraseRecognizedEventArgs args){
        HandleCorrect();
        Msg.text = "Correct!";
        SelectedPlayer.current_speech_pass++;
        StartCoroutine(Reset());
    }

    IEnumerator Reset(){
        WordDisplay.ResetWordDisplay();
        yield return new WaitForSeconds(endDisplayTime);
        WordDisplay.DeleteWordDisplay();
        yield return StartCoroutine(DelayAndDestroy());
    }

    protected override void HandleIncorrect()
	{
        StopRecognizer();
		SelectedPlayer.current_speech_fail++;
	}

    private void OnApplicationQuit()
    {
        StopRecognizer();
    }

    private void StopRecognizer()
    {
        if (recognizer != null && recognizer.IsRunning)
        {   
            recognizer.OnPhraseRecognized -= Recognizer_OnPhraseRecognized;
            recognizer.Stop();
            recognizer.Dispose();
        }
    }
}
