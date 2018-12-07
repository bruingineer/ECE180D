using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//This is wrapper class to the DisplayWord class
//to hide the details and make it easier to use
public class HandleWordDisplay : MonoBehaviour {

    public void WhichWord(string w)
    {
        SetWord(w);
    }

    // Use this for initialization
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
    
    public GameObject initPosition;

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
        print(word);
        int offset = 0;
        foreach (char c in this.word)
        {
            GameObjectStruct myGameObjectStruct = new GameObjectStruct { myGameObject = LoadLetter.GetLetter(c) };
            myGameObjectStruct.myGameObject.transform.localScale -= new Vector3(0.5f, 0.5f, 0);
            myGameObjectStruct.myGameObject.transform.SetParent(this.transform);
            //GameObjectStruct obj = new GameObjectStruct { myGameObject =  gobj};
            Vector3 pos = initPosition.transform.position;
            pos[0] = pos[0] + 42 * offset;
            offset++;
            myGameObjectStruct.myGameObject.transform.position = pos;
            myGameObjectStruct.initPos = pos;
            initPositions.Add(pos);
            wordList.Add(myGameObjectStruct);
        }

        //Now Randomize initPostions
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

public struct GameObjectStruct
{
    public GameObject myGameObject;
    public Vector3 initPos;
}
