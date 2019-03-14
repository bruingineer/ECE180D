using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


public class SpeechList : MonoBehaviour
{
    
    public static IDictionary <string, string> triviaDictionay = new Dictionary<string, string>()
    {
        {"Washington", "Who was the first US President?"},
        {"light", "What is the UC motto?"},
        {"murder","What do you call a flock of crows"},
        {"wooden","Who is the most successful UCLA coach?"},
        {"rose", "Where does UCLA play home football games?"},
        {"right","Up, down, left, _____" },
        {"pizza","Round and has pepperoni." },
        {"down", "Opposite of up?" },
        {"exit", "What's the opposite of enter?" },
        {"water", "What is the ocean made of?" }
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
        "boat",
        "look",
        "outreach",
        "engineer",
        "national",
        "week",
        "university",
        "crazy",
        "winner",
        "chicken",
        "loser",
        "potato",
        "dinner",
        "active",
        "super",
        "random",
        "country",
        "dolphin",
        "between",
        "snake"

    // "en<uppercase><color=\"blue\">g</color=\"blue\"><uppercase>ineer"
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


