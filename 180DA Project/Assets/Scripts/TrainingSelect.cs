using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TrainingSelect : MonoBehaviour {

    public Text training_suggestion;
    public Button trainingButton;
    public Dropdown m_Dropdown;
    List<string> m_DropOptions = new List<string>();
    bool populated = false;
   

    // Use this for initialization
    void Start () {
        trainingButton = GetComponent<Button>();
        trainingButton.onClick.AddListener(ChooseScene);

        m_Dropdown.ClearOptions();
    }
	
	// Update is called once per frame
	void Update () {
		if (training_suggestion.text != "" && !populated)
        {
            string txt = training_suggestion.text;
            if (txt.Contains("Lasers"))
            {
                m_DropOptions.Add("Laser Training");
            }

            if (txt.Contains("Speech Training"))
            {
                m_DropOptions.Add("Speech");
            }

            if (txt.Contains("Gestures"))
            {
                m_DropOptions.Add("Gesture Training");
            }

            m_Dropdown.AddOptions(m_DropOptions);
            populated = true;
        }
	}

    void ChooseScene()
    {
        string choice = m_DropOptions[m_Dropdown.value];
        SceneManager.LoadScene(choice);
    }
}
