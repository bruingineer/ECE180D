using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class selectTrainingScene : MonoBehaviour {

    public Text training_suggestion;
    public Button trainingButton;

    // Use this for initialization
    void Start () {
        trainingButton = GetComponent<Button>();
        trainingButton.onClick.AddListener(ChooseScene);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void ChooseScene()
    {
        string txt = training_suggestion.text;
        if ( txt != null)
        {
            if (txt.Contains("gesture"))
            {
                Debug.Log("Changing to gesture training");
                SceneManager.LoadScene("Gesture Training");
            }

            else if (txt.Contains("speech"))
            {
                Debug.Log("Changing to speech training");
                SceneManager.LoadScene("Speech Training");
            }

            else if (txt.Contains("dodging"))
            {
                Debug.Log("Changing to laser training");
                SceneManager.LoadScene("Laser Training");
            }
        }
    }
}
