using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisplayText : MonoBehaviour {

    public Text results;

    public void UpdateText(string text)
    {
        results.text = "You said: <b>" + text + "</b>";
    }
}
