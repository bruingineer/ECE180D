using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class menuScene : MonoBehaviour {

	public void changeMenuScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public static void ChangeScenefromScript(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
    
}

