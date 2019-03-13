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

        if (!SelectedPlayer.laser_training) message += "\tLaser Training\n";
        if (!SelectedPlayer.gesture_training) message += "\tGesture Training\n";
        if (!SelectedPlayer.unscramble_training) message += "\tScramble Training\n";
        if (!SelectedPlayer.trivia_training) message += "\tTrivia Training";

        //if player has completed all training, change to main level scene,
        //else give instructions
        if (message == orig) SceneManager.LoadScene("Main Level");
        else txt.text = message;
    }
}
