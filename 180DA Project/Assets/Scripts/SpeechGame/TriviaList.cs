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
        {"let there be light", "What is the UC motto?"},
        {"murder","What do you call a flock of crows"},
        {"john wooden","Who is the most successful UCLA coach?"},
        {"rose bowl", "Where does UCLA play home football games?"},
        {"ohm's law", "What do you call: I = V / R"}
    };

    public static int size = triviaDictionay.Count; 
    public static void getQuestion(ref string ques, ref string ans){
        int r = Random.Range(0, size);
        ans = triviaDictionay.Keys.ElementAt(r);
        ques = triviaDictionay[ans];
    }

    public static void getQuestionNumber(ref string ques, ref string ans, int i = -1){
        
        if (i==-1){
            getQuestion(ref ques, ref ans);
        }
        else{
            ans = triviaDictionay.Keys.ElementAt(i);
            ques = triviaDictionay[ans];
        }
    }
}
