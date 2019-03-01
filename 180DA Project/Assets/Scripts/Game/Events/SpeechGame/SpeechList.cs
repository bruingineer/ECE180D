using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


public class SpeechList : MonoBehaviour
{
    
    public static IDictionary <string, string> triviaDictionay = new Dictionary<string, string>()
    {
        {"george washington", "Who was the first US President?"},
        //{"thursday", "What day is it?"},
        {"let there be light", "What is the UC motto?"},
        {"murder","What do you call a flock of crows"},
        {"john wooden","Who is the most successful UCLA coach?"},
        {"rose bowl", "Where does UCLA play home football games?"},
        {"hogwarts","Name the school that Harry Potter attended" },
        //{"woody","What is the name of the toy cowboy in Toy Story?" },
        {"pizza","What's round and has pepperoni?" },
        {"circumference", "What is the distance around a circle called?" },
        {"exit", "What's the opposite of enter?" }
        //{"ohm's law", "What do you call: I = V / R"}
        //total = 9
    };

    //public static string[] WordList_ = new string[] {
    public static List<string> WordList_ = new List<string>()
    {
        "crime",
        "fame",
        "apple",
    //         //"mouse", hard to detect
        "lime",
        "kid",
        "make",
    //     //"pot", // may not work
        "pick",
        "bit",
        "luck",
        "boat",
        "look",
        "outreach",
        "engineer",
        "national",
        "week",

    //     //"lake", ambiguous
        "snake"
    //     "nap" ambiguous
    //     "slime" ambiguous
    //total 16

    // "en<uppercase><color=\"blue\">g</color=\"blue\"><uppercase>ineer"
    };

    public static int ListSize = WordList_.Count();

    public static string getWord(){
        return WordList_[Random.Range(0,ListSize)];
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


