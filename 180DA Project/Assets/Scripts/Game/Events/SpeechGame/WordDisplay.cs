using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;

public class WordDisplay 
{
    public TextMeshProUGUI WordText;
    public string word_str;
    private string orginal_scramble;
    public void SetWordDisplay(string w){
        //WordText = GameObject.FindWithTag("task").GetComponent<TextMeshProUGUI>();
        //WordText.text = "";
        word_str = w;
        while ( (WordText.text = Shuffle.StringMixer(w)) == word_str)
            ;
        orginal_scramble = WordText.text;
    }

    public void ResetWordDisplay(){
        WordText.text = word_str;
    }

    public void DeleteWordDisplay(){
        WordText.text = "";
    }


    public void MakeBlink() {
        int r = UnityEngine.Random.Range(0, orginal_scramble.Length);
        //Debug.Log(word_str);
        //Debug.Log(orginal_scramble);
        //Debug.Log(r);
        //Debug.Log(orginal_scramble.Length);
        if ( (r != 0) && (r != orginal_scramble.Length - 1))
            WordText.text = orginal_scramble.Substring(0, r - 1) + " " + orginal_scramble.Substring(r+1);
        else if ( r == 0)
            WordText.text = " " + orginal_scramble.Substring(1);
        else // r == original_scrabmle.Length - 1
            WordText.text = orginal_scramble.Substring(0, orginal_scramble.Length - 1);
        
        // yield return new WaitForSeconds(repeatRate);
    }

    
    public void MakeWordBlink(){
        if (WordText.text != orginal_scramble)
            WordText.text = orginal_scramble;
        else
            WordText.text = "";
    }

    public void SetFirstLetterClue(){
        char firstLetter = word_str[0];
        //Debug.Log(firstLetter);
        string beg = orginal_scramble.Substring(0, orginal_scramble.IndexOf(firstLetter));
        string end = orginal_scramble.Substring(orginal_scramble.IndexOf(firstLetter)+1);
        //Debug.Log(beg); Debug.Log(end);
        //Debug.Log("setting modified word");
        WordText.text = beg + "<color=\"blue\">" + firstLetter + "</color>" + end;
    }
}



class Shuffle
{
    static System.Random rnd = new System.Random();

    static void Fisher_Yates(int[] array)
    {
        int arraysize = array.Length;
        int random;
        int temp;

        for (int i = 0; i < arraysize; i++)
        {
            random = i + (int)(rnd.NextDouble() * (arraysize - i));

            temp = array[random];
            array[random] = array[i];
            array[i] = temp;
        }
    }

    public static string StringMixer(string s)
    {
        string output = "";
        int arraysize = s.Length;
        int[] randomArray = new int[arraysize];

        for (int i = 0; i < arraysize; i++)
        {
            randomArray[i] = i;
        }

        Fisher_Yates(randomArray);

        for (int i = 0; i < arraysize; i++)
        {
            output += s[randomArray[i]];
        }

        return output;
    }
}