using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Windows.Speech;

public class SpeechRecognitionEngine : MonoBehaviour
{
    public Text timeLeft;
    public HandleWordDisplay HandleWordDisplay;
    public ConfidenceLevel confidence = ConfidenceLevel.Medium;
    protected static PhraseRecognizer recognizer;
    private bool isCorrect;
    public Text scramble;
    public string[] keywords = { "" };
    private Player m_player;

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
    private void Start()
    {
        m_player = GameObject.Find("Player").GetComponent<Player>();
        GameObject time = GameObject.FindWithTag("timer");
        scramble.transform.position = time.transform.position + new Vector3(-450, 0, 0);
        //TimeLeft.transform.position = scramble.transform.position + new Vector3(300,0,0);
        HandleWordDisplay.InitPosition = scramble.transform.position - new Vector3(80,35,0);
        isCorrect = false;
        ChooseRandWord();
        HandleWordDisplay.Display();
        if (keywords != null)
        {
            recognizer = new KeywordRecognizer(keywords, confidence);
            recognizer.OnPhraseRecognized += Recognizer_OnPhraseRecognized;
            recognizer.Start();
        }
        
        StartCoroutine("Timer");
        
    }

    void makeBlink()
    {
        HandleWordDisplay.makeBlink();
    }

    void Update()
    {
        InvokeRepeating("makeBlink", 0, 1);
        if (Player.isDead)
            Destroy(gameObject);
    }

    private void Recognizer_OnPhraseRecognized(PhraseRecognizedEventArgs args)
    {
        if(!isCorrect) {
            StopCoroutine("Timer");
            isCorrect = true;
            scramble.text = "Correct!";
            Speech_MiniGame.curCorrect++;
            m_player.MovePlayer();
            StartCoroutine(Reset());
            SelectedPlayer.current_speech_pass++;
            Debug.Log("current_speech_pass++");
        }
    }

    IEnumerator Reset()
    {
        HandleWordDisplay.ResetDisplay();
        //Think we can display the word for a few second then remove it from the display
        yield return new WaitForSeconds(2);
        //To remove the word from the display
        HandleWordDisplay.RemoveDisplay();

        //to stop the PhraseRecognizer
        StopRecognizer();
        PlayerEvents.eventOn = false;
        Speech_MiniGame.eventOn = false;
        timeLeft.text = "";
        Destroy(gameObject);
    }

    public IEnumerator Timer() 
	{
        timeLeft = GameObject.FindWithTag("timer").GetComponent<Text>();

        float duration = 11f;
        //Change time allowed to perform gesture based on difficulty selected
        if (SelectedPlayer.current_difficulty == "easy") duration = 11f;
        else if (SelectedPlayer.current_difficulty == "medium") duration = 8f;
        else if (SelectedPlayer.current_difficulty == "hard") duration = 5f;

        while (duration >= 0)
            {   
                duration -= Time.deltaTime;
                int integer = (int)duration;
                if (integer >= 1)
                    timeLeft.text = integer.ToString();
                else
                {
                    StopRecognizer();
                    timeLeft.text = "Time's Up";
                }
                yield return null;
            }
            
            yield return new WaitForSeconds(1);
            Speech_MiniGame.curCorrect = 0;
            PlayerEvents.eventOn = false;
            Speech_MiniGame.eventOn = false;
            timeLeft.text = "";
            if (!isCorrect) {
                SelectedPlayer.current_speech_fail++;
                Debug.Log("current_speech_fail++");
                Destroy(timeLeft);
                Destroy(gameObject);
            }

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