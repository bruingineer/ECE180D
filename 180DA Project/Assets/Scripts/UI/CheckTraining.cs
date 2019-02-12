using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine;

public class CheckTraining : MonoBehaviour {

    public Text txt;
    public Button start;

	//Use this for initialization
	void Start () {
		start = GetComponent<Button>();
        start.onClick.AddListener(checkTraining);
	}

    //Unlocks Main Level once training is complete
    void checkTraining()
    {
        string message = "To unlock the Main Level, please complete the following:\n";
        string orig = message;

        if (!SelectedPlayer.laser_training) message += "Laser Training\n";
        if (!SelectedPlayer.gesture_training) message += "Gesture Training\n";
        if (!SelectedPlayer.speech_training) message += "Speech Training";

        //if player has completed all training, change to main level scene,
        //else give instructions
        if (message == orig) SceneManager.LoadScene("Main Level");
        else txt.text = message;
    }
}
