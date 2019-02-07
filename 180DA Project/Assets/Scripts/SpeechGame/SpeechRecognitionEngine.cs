using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows.Speech;
using UnityEngine.UI;

public class SpeechRecognitionEngine : Event
{
    // Objects
    public HandleWordDisplay HandleWordDisplay;
    public ConfidenceLevel confidence = ConfidenceLevel.Medium;
    protected static PhraseRecognizer recognizer;
    public Text scramble;
    public string[] keywords = { "" };
    private float endDisplayTime = 1.1f;
    

    //This will update the keywords that the speech recognizer will recognize
    //For the list of avaiable words see WordList.cs

    //To select which word
    public void WhichWord(int i)
    {
        keywords[0] = WordList.WordList_[i];
        HandleWordDisplay.WhichWord(keywords[0]);
    }

    //To randomize which word
    public void ChooseRandWord()
    {
        System.Random rnd = new System.Random();
        keywords[0] = WordList.WordList_[rnd.Next(WordList.WordList_.Length)];
        //print(keywords[0]);
        HandleWordDisplay.WhichWord(keywords[0]);
    }

    //Can maybe use enable/disable
    

    protected override void Initialize()
    {
        scramble.transform.position = timeLeft.transform.position + new Vector3(-450, 0, 0);
        HandleWordDisplay.InitPosition = scramble.transform.position - new Vector3(80, 35, 0);
    }

    protected override void SetUpEvent()
	{
		ChooseRandWord();
        HandleWordDisplay.Display();
        if (keywords != null)
        {
            recognizer = new KeywordRecognizer(keywords, confidence);
            recognizer.OnPhraseRecognized += Recognizer_OnPhraseRecognized;
            recognizer.Start();
        }
	}

    protected override IEnumerator MakeTextBlink()
    {
        yield return HandleWordDisplay.MakeBlink(repeatRate);
    }

    private void Recognizer_OnPhraseRecognized(PhraseRecognizedEventArgs args)
    {
            timerPaused = true;
            m_player.MovePlayer();
            scramble.text = "Correct!";
            StopRecognizer();
            SelectedPlayer.current_speech_pass++;
            StartCoroutine(Reset());
    }

    IEnumerator Reset()
    {
        HandleWordDisplay.ResetDisplay();
        //Think we can display the word for a few second then remove it from the display
        yield return new WaitForSeconds(endDisplayTime);
        //To remove the word from the display
        HandleWordDisplay.RemoveDisplay();
        //to stop the PhraseRecognizer
        yield return StartCoroutine(DelayAndEndTimer());
    }

    protected override IEnumerator HandleIncorrect()
	{
        StopRecognizer();
		SelectedPlayer.current_speech_fail++;
        yield return StartCoroutine(DelayAndEndTimer());
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