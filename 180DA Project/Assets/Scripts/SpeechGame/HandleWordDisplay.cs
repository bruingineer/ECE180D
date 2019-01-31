using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HandleWordDisplay : MonoBehaviour {

    public Vector3 InitPosition;
    public static int Offset = 42;

    public struct GameObjectStruct
{
    public GameObject myGameObject;
    public Vector3 initPos;
}

    public void WhichWord(string w)
    {
        SetWord(w);
    }

    public void Display()
    {
        LoadWord();
    }
        
    public void ResetDisplay()
    {
        ResetWord();
    }

    public void RemoveDisplay()
    {
        DeleteWord();
    }

    //Moved from DisplayWord.cs in order to reduce the number of Gameobjects needed
    private string word;
    private List<GameObjectStruct> wordList = new List<GameObjectStruct>();
    private List<Vector3> initPositions = new List<Vector3>();
    

    public void SetWord(string w)
    {
        word = w;
    }

    public void LoadWord()
    {
        word = word.ToUpper();
        Set_wordDict();
    }

    private void Set_wordDict()
    {
        Debug.Log("Word is: " + word);
        int letter = 0;
        foreach (char c in this.word)
        {
            GameObjectStruct myGameObjectStruct = new GameObjectStruct { myGameObject = LoadLetter.GetLetter(c) };
            myGameObjectStruct.myGameObject.transform.localScale -= new Vector3(0.5f, 0.5f, 0);
            myGameObjectStruct.myGameObject.transform.SetParent(this.transform);
            Vector3 pos = InitPosition;
            pos[0] = pos[0] + Offset * letter;
            letter++;
            myGameObjectStruct.myGameObject.transform.position = pos;
            myGameObjectStruct.initPos = pos;
            initPositions.Add(pos);
            wordList.Add(myGameObjectStruct);
        }

        while (!IsRandomized())
        { Randomize(); }
        
    }

    public IEnumerator MakeBlink(float repeatRate)
    {
        int r = Random.Range(0, initPositions.Count); //choose a random index
        if (wordList[r].myGameObject.activeSelf)
            wordList[r].myGameObject.SetActive(false);
        else 
            wordList[r].myGameObject.SetActive(true);
        yield return new WaitForSeconds(repeatRate);
    }

    public void Randomize()
    {
        for (int i = 0; i < initPositions.Count; i++)
        {
            Vector3 temp = initPositions[i];
            int randomIndex = Random.Range(i, initPositions.Count);
            initPositions[i] = initPositions[randomIndex];
            initPositions[randomIndex] = temp;
        }

        for (int i = 0; i < wordList.Count; i++)
        {
            wordList[i].myGameObject.transform.position = initPositions[i];
        }
    }

    public bool IsRandomized()
    {
        for (int i = 0; i < initPositions.Count; i++)
        {
            if (wordList[i].myGameObject.transform.position != wordList[i].initPos)
            { return true; }
        }
        return false;
    }

    //Reset the original position of the word
    public void ResetWord()
    {
        for (int i = 0; i < initPositions.Count; i++)
        {
            wordList[i].myGameObject.transform.position = wordList[i].initPos;
        }
    }

    public void DeleteWord()
    {
        for (int i = 0; i < wordList.Count; i++)
        {
            Destroy(wordList[i].myGameObject);
        }
        wordList.Clear();
    }
}
