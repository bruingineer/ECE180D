using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


public class SpeechList : MonoBehaviour
{
    
    public static IDictionary <string, string> triviaDictionay = new Dictionary<string, string>()
    {
        {"cat", "Animal that meows?"},
        {"down", "Opposite of up?"},
        {"exit","Opposite of enter?"},
        {"London","Capital of England?"},
        {"Lakers", "Lakers or Clippers?"},
        {"right","Opposite of left?" },
        {"pizza","Round and has pepperoni?" },
        {"Friday", "Last day of the week?" },
        {"back", "Opposite of front?" },
        {"two", " 14 % 6?"},
        {"ten", "2 + 16 / 2?"}
    };

    public static string[] WordList_ = new string[] {
        "cat",
        "down",
        "exit",
        "London",
        "Lakers",
        "right",
        "pizza",
        "Friday",
        "back",
        "two",
        "ten",


        "crime",
        "make",
        "pick",
        "bit",
        "engineer",
        "national",
        "university",
        "crazy",
        "winner",
        "chicken",
        "loser",
        "potato",
        "dinner",
        "active",
        "country",
        "dolphin",
        "between"
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


