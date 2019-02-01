using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


public class TriviaList : MonoBehaviour
{
    
    public static IDictionary <string, string> triviaDictionay = new Dictionary<string, string>()
    {
        {"george washington", "Who was the first US President?"},
        //{"thursday", "What day is it?"},
        {"let there be light", "What is the UC motto?"}
    };

    public static int size = triviaDictionay.Count; 
    public static void getQuestion(ref string ques, ref string ans){
        int r = Random.Range(0, size);
        ans = triviaDictionay.Keys.ElementAt(r);
        ques = triviaDictionay[ans];
    }
}
