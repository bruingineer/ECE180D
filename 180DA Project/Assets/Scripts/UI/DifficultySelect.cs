using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DifficultySelect : MonoBehaviour {

    public Button easy;
    public Button medium;
    public Button hard;
    
	void Start () {
        //set default to easy
        easy.Select();
        
        easy.onClick.AddListener(delegate {changeSelection("easy");});
        medium.onClick.AddListener(delegate { changeSelection("medium"); });
        hard.onClick.AddListener(delegate { changeSelection("hard"); });
    }

    void changeSelection(string button)
    {
        //Pressing difficulty button will just change static SelectedPlayer file's difficulty variable
        //Other programs such as gesture and speech recognition should read this variable and change the 
        //speed or other settings through if statements
        if (button == "easy")
        {
            Debug.Log("Difficulty set to Easy");
            SelectedPlayer.current_difficulty = "easy";
        }
        if (button == "medium")
        {
            //Only allow medium if player has unlocked this difficulty
            if (SelectedPlayer.suggested_difficulty == "easy") easy.Select();
            else
            {
                Debug.Log("Difficulty set to Medium");
                SelectedPlayer.current_difficulty = "medium";
            }
        }
        if (button == "hard")
        {
            //Only allow hard if player has unlocked this difficulty
            if (SelectedPlayer.suggested_difficulty == "easy") easy.Select();
            else if (SelectedPlayer.suggested_difficulty == "medium") medium.Select();
            else
            {
                Debug.Log("Difficulty set to Hard");
                SelectedPlayer.current_difficulty = "hard";
            }
        }
    }
}
