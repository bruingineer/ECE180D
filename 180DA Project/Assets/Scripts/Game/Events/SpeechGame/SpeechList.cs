using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


public class SpeechList : MonoBehaviour
{
    
    public static IDictionary <string, string> triviaDictionay = new Dictionary<string, string>()
    {
        {"sacramento", "Capital of California?"},
        {"down", "Opposite of up?"},
        {"exit","Opposite of enter?"},
        {"london","Capital of England?"},
        {"lakers", "Lakers or Clippers?"},
        {"light","Opposite of dark?" },
        {"pizza","Round and has pepperoni?" },
        {"friday", "Last day of the week?" },
        {"east", "Opposite of west?" }
    };

    public static string[] WordList_ = new string[] {
        "washington",
        "light",
        "murder",
        "wooden",
        "rose",
        "right",
        "pizza",
        "down",
        "exit",
        "crime",
        "fame",
        "apple",
        "lime",
        "kid",
        "make",
        "pick",
        "bit",
        "luck",
        "boat",
        "look",
        "outreach",
        "engineer",
        "national",
        "week",
        "snake"
    };

    public static int ListSize = WordList_.Count();

    public static string getWord(int wordIndex){
        int index = wordIndex == -1 ? Random.Range(0,ListSize) : wordIndex;
        return WordList_[index];
    }

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


