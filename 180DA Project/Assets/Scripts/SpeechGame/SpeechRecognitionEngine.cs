using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows.Speech;
using UnityEngine.UI;
using TMPro;

public class SpeechRecognitionEngine : Event 
{
    private WordDisplay WDisplay;
    public TextMeshProUGUI Msg;
    public ConfidenceLevel confidence = ConfidenceLevel.Medium;
    protected static PhraseRecognizer recognizer;
    public string[] keywords = {""};
    private float endDisplayTime = 1.1f;
    protected override void Initialize(){
        WDisplay = new WordDisplay();
        WDisplay.WordText = GameObject.FindWithTag("word").GetComponent<TextMeshProUGUI>();
        WDisplay.WordText.text = "";
        Msg = GameObject.FindWithTag("msg").GetComponent<TextMeshProUGUI>();
        Msg.text = "";
        SetUp();
    }
    protected override void SetUpEvent(){
        //choose a random word
        System.Random rnd = new System.Random();
        keywords[0] = WordList.WordList_[rnd.Next(WordList.WordList_.Length)];
        WDisplay.SetWordDisplay(keywords[0]);
        Msg.text = "Unscramble This:";
        if (keywords != null){
            recognizer = new KeywordRecognizer(keywords, confidence);
            recognizer.OnPhraseRecognized += Recognizer_OnPhraseRecognized;
            recognizer.Start();
            PhraseRecognitionSystem.Restart();
        }
        if (PhraseRecognitionSystem.Status == SpeechSystemStatus.Running){
            Debug.Log("starting another scrabmle");
        }
        Debug.Log(WDisplay.word_str);
    }

    protected override IEnumerator MakeTextBlink(){
        // WDisplay.MakeBlink();
        // yield return new WaitForSeconds(repeatRate);
        while(!timerPaused){
            Debug.Log("making word blink");
            WDisplay.MakeWordBlink();
			yield return new WaitForSeconds(repeatRate/1.5f);
        }

    }

    private void Recognizer_OnPhraseRecognized(PhraseRecognizedEventArgs arg){
        timerPaused = true;
        Msg.text = "Correct!";
        StopRecognizer();
        // potentially change selected player
        SelectedPlayer.current_speech_pass++;
        HandleCorrect();
        StartCoroutine("Reset");            
    }

    IEnumerator Reset(){
        WDisplay.ResetWordDisplay();
        yield return new WaitForSeconds(endDisplayTime);
        WDisplay.DeleteWordDisplay();
        Msg.text = "";
        yield return Delay();
        StopCoroutine("MakeTextBlink");
        eventCorrect = true;
        yield return null;
    }

    protected override IEnumerator HandleIncorrect(){
        timerPaused = true;
        StopRecognizer();
        HandleIncorrectMiniGame();
        SelectedPlayer.current_speech_fail++;
        WDisplay.DeleteWordDisplay();
        Msg.text = "";
        yield return Delay();
    }

    private void OnApplicationQuit(){
        StopRecognizer();
    }

    private void StopRecognizer(){
        if (recognizer != null && recognizer.IsRunning){
            recognizer.OnPhraseRecognized -= Recognizer_OnPhraseRecognized;
            recognizer.Stop();
            recognizer.Dispose();
            PhraseRecognitionSystem.Shutdown();
        }    
    }

    protected virtual void HandleCorrect() 
    {
        m_player.MovePlayer();
    }
    protected virtual void HandleIncorrectMiniGame() {}

}

public class SpeechRecognitionEngine_Minigame : SpeechRecognitionEngine {
	protected override void HandleCorrect()
	{
		GameState_Event_Minigame.curCorrect++;
	}

	protected override void HandleIncorrectMiniGame()
	{
		GameState_Event_Minigame.curCorrect = 0;
	}

    protected override void SetUp() {}
}

