using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//This is wrapper class to the DisplayWord class
//to hide the details and make it easier to use
public class HandleWordDisplay : MonoBehaviour {


    public DisplayWord DisplayWord;

    public void WhichWord(string w)
    {
        print(w);
        DisplayWord.SetWord(w);
    }

    // Use this for initialization
    public void Display()
    {
        DisplayWord.LoadWord();
    }
        
    public void ResetDisplay()
    {
        DisplayWord.ResetWord();
    }

    public void RemoveDisplay()
    {
        DisplayWord.DeleteWord();
    }

}
