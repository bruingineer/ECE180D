using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisplayWord : MonoBehaviour
{
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
            myGameObjectStruct.myGameObject.transform.SetParent(this.transform);
            //GameObjectStruct obj = new GameObjectStruct { myGameObject =  gobj};
            Vector3 pos = initPosition.transform.position;
            pos[0] = pos[0] + 90 * offset;
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
