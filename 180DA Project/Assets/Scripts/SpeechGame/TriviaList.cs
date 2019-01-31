using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


public class TriviaList : MonoBehaviour
{
    public static int size = 2;
    public static IDictionary <string, string> triviaDictionay = new Dictionary<string, string>()
    {
        {"George Washingtion", "Who was the first US President?"},
        {"Court of Sciences", "Patio in front of Boelter Hall is called?"}
    };

    public static void getQuestion(ref string question, ref string answer){
        int r = Random.Range(0, size);
        question = triviaDictionay.Keys.ElementAt(r);
        answer = triviaDictionay[question];
    }
}
