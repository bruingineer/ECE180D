using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows.Speech;
using UnityEngine.UI;
using TMPro;

public class SpeechRecognitionEngine2 : Event 
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
    }
    protected override void SetUpEvent(){
        //choose a random word
        //Debug.Log("SR2.Start->SetUp");
        //Initialize();
        //SetUpCoroutines();
        System.Random rnd = new System.Random();
        keywords[0] = WordList.WordList_[rnd.Next(WordList.WordList_.Length)];
        WDisplay.SetWordDisplay(keywords[0]);
        Msg.text = "Unscramble This:";
        if (keywords != null){
            recognizer = new KeywordRecognizer(keywords, confidence);
            recognizer.OnPhraseRecognized += Recognizer_OnPhraseRecognized;
            recognizer.Start();
        }
        Debug.Log(WDisplay.word_str);
    }

    protected override IEnumerator MakeTextBlink(){
        WDisplay.MakeBlink();
        yield return new WaitForSeconds(repeatRate);
    }

    private void Recognizer_OnPhraseRecognized(PhraseRecognizedEventArgs arg){
        timerPaused = true;
        m_player.MovePlayer();
        Msg.text = "Correct!";
        StopRecognizer();
        SelectedPlayer.current_speech_pass++;
        HandleCorrectMiniGame();
        StartCoroutine(Reset());            
    }

    IEnumerator Reset(){
        WDisplay.ResetWordDisplay();
        yield return new WaitForSeconds(endDisplayTime);
        WDisplay.DeleteWordDisplay();
        Msg.text = "";
        yield return StartCoroutine(Delay());
        StopCoroutine(StartTimer());
    }

    protected override IEnumerator HandleIncorrect(){
        StopRecognizer();
        HandleIncorrectMiniGame();
        SelectedPlayer.current_speech_fail++;
        yield return StartCoroutine(Delay());
    }

    private void OnApplicationQuit(){
        StopRecognizer();
    }

    private void StopRecognizer(){
        if (recognizer != null && recognizer.IsRunning){
            recognizer.OnPhraseRecognized -= Recognizer_OnPhraseRecognized;
            recognizer.Stop();
            recognizer.Dispose();
        }    
    }

    protected virtual void HandleCorrectMiniGame() {}
    protected virtual void HandleIncorrectMiniGame() {}

}

public class SpeechRecognitionEngine_Minigame : SpeechRecognitionEngine2 {
	protected override void HandleCorrectMiniGame()
	{
		GetComponent<PlayerEvents_Gesture_Minigame>().curCorrect++;
	}

	protected override void HandleIncorrectMiniGame()
	{
		GetComponent<PlayerEvents_Gesture_Minigame>().curCorrect = 0;
	}
}



// public class SpeechRecognitionEngine2 : Event
// {
//     private WordDisplay WDisplay;
//     public TextMeshProUGUI Msg;
//     public ConfidenceLevel confidence = ConfidenceLevel.Medium;
//     protected static PhraseRecognizer recognizer;
//     public string[] keywords = {""};
//     private float endDisplayTime = 1.1f;

//     private void SetDisplay(){
//         //choose a random word
//         System.Random rnd = new System.Random();
//         keywords[0] = WordList.WordList_[rnd.Next(WordList.WordList_.Length)];
//         WDisplay.SetWordDisplay(keywords[0]);
//         //Msg = GameObject.FindGameObjectWithTag("msg").GetComponent<TextMeshProUGUI>();
//         //Msg.text = "";
//         Msg.text = "Unscramble This:";
//         // Task = GameObject.FindGameObjectWithTag("task").GetComponent<TextMeshProUGUI>();
//     }

//     protected override void StartRunningTask(){
//         SetDisplay();
//         if (keywords != null){
//             recognizer = new KeywordRecognizer(keywords, confidence);
//             recognizer.OnPhraseRecognized += Recognizer_OnPhraseRecognized;
//             recognizer.Start();
//         }
//     }

//     protected override void StopRunningTask(){
//         StopRecognizer();
//     }

//     protected override void SetUp(){
//         WDisplay = new WordDisplay();
//         WDisplay.WordText = GameObject.FindGameObjectWithTag("word").GetComponent<TextMeshProUGUI>();
//         WDisplay.WordText.text = "";
//         Msg = GameObject.FindGameObjectWithTag("msg").GetComponent<TextMeshProUGUI>();
//         Msg.text = "";
//     }

//     // protected override void SetUp()
//     // {
//     //     SetDisplay();
//     //     if (keywords != null){
//     //         recognizer = new KeywordRecognizer(keywords, confidence);
//     //         recognizer.OnPhraseRecognized += Recognizer_OnPhraseRecognized;
//     //         recognizer.Start();
//     //     }
        
//     // }

//     //TODO: Implement
//     protected override IEnumerator MakeTextBlink(){
//         WDisplay.MakeBlink();
//         yield return new WaitForSeconds(repeatRate);
//     } 

//     void Update(){
//         if (Player.isDead){
//             //Destroy(gameObject);
//             Msg.text = "";
//             // Task.text = "";
//         }
//         //StartCoroutine(MakeTextBlink());
//     }

//     private void Recognizer_OnPhraseRecognized(PhraseRecognizedEventArgs args){
//         HandleCorrect();
//         Msg.text = "Correct!";
//         SelectedPlayer.current_speech_pass++;
//         StartCoroutine(Reset());
//     }

//     IEnumerator Reset(){
//         WDisplay.ResetWordDisplay();
//         yield return new WaitForSeconds(endDisplayTime);
//         WDisplay.DeleteWordDisplay();
//         yield return StartCoroutine(DelayAndDestroy());
//     }

//     protected override void HandleIncorrect()
// 	{
//         StopRecognizer();
// 		SelectedPlayer.current_speech_fail++;
// 	}

//     private void OnApplicationQuit()
//     {
//         StopRecognizer();
//     }

//     private void StopRecognizer()
//     {
//         if (recognizer != null && recognizer.IsRunning)
//         {   
//             recognizer.OnPhraseRecognized -= Recognizer_OnPhraseRecognized;
//             recognizer.Stop();
//             recognizer.Dispose();
//         }
//     }
// }