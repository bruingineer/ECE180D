using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Windows.Speech;


public class SpeechRecognitionEngine : MonoBehaviour
{
    public static int numSuccess;
    public static int numFails;
        //0 = no change      //1 = success      //2 = fail
    public static int status; // will be checked per frame

    public Text TimeLeft;

    public HandleWordDisplay HandleWordDisplay;
    public ConfidenceLevel confidence = ConfidenceLevel.Medium;
    protected static PhraseRecognizer recognizer;
    private bool isCorrect;

    public static Vector3 SeaRef;

    public Text scramble;
    protected string word = "";
    public string[] keywords = { "" };

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
        GameObject sea = GameObject.FindWithTag("SeaRef");
        scramble.transform.position = sea.transform.position + new Vector3(60, 300, 0);
        TimeLeft.transform.position = scramble.transform.position + new Vector3(300,0,0);
        HandleWordDisplay.InitPosition = scramble.transform.position - new Vector3(35,20,0);

        isCorrect = false;
        ChooseRandWord();
        HandleWordDisplay.Display();
        if (keywords != null)
        {
            recognizer = new KeywordRecognizer(keywords, confidence);
            recognizer.OnPhraseRecognized += Recognizer_OnPhraseRecognized;
            recognizer.Start();
            //print("Recognizer Started");  //for debugging
        }
        StartCoroutine("Timer");
    }

    void Update()
    {
        if (Player.isDead)
            Destroy(gameObject);
    }

    private void Recognizer_OnPhraseRecognized(PhraseRecognizedEventArgs args)
    {
        if(!isCorrect) {
            StopCoroutine("Timer");
            isCorrect = true;
            scramble.text = "Correct!";
            PlayerMQTT_X.playerMoved = true;
            StartCoroutine(Reset());
            numSuccess++;
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
        Destroy(gameObject);
        
    }

    public IEnumerator Timer() 
	{
            float duration = 8f;
            while(duration >= 0)
            {   
                duration -= Time.deltaTime;
                int integer = (int)duration;
                if (integer >= 1)
                    TimeLeft.text = integer.ToString();
                else
                {
                    StopRecognizer();
                    TimeLeft.text = "Time's Up";
                }
                yield return null;
            }
            
            yield return new WaitForSeconds(1);
            PlayerEvents.eventOn = false;
            
            if (!isCorrect) { 
                numFails++;
                Destroy(TimeLeft);
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
            //print("Recognizer Stopped"); //for debugging
        }
        
    }
}