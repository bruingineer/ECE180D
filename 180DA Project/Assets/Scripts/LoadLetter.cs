using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadLetter {

    // public static GameObject letter;

    public static GameObject GetLetter(char letter)
    {
        Sprite mySprite = null;

        switch (letter)
        {
            case 'A': mySprite = Resources.Load<Sprite>("Letters/A"); break;
            case 'B': mySprite = Resources.Load<Sprite>("Letters/B"); break;
            case 'C': mySprite = Resources.Load<Sprite>("Letters/C"); break;
            case 'D': mySprite = Resources.Load<Sprite>("Letters/D"); break;
            case 'E': mySprite = Resources.Load<Sprite>("Letters/E"); break;
            case 'F': mySprite = Resources.Load<Sprite>("Letters/F"); break;
            case 'G': mySprite = Resources.Load<Sprite>("Letters/G"); break;
            case 'H': mySprite = Resources.Load<Sprite>("Letters/H"); break;
            case 'I': mySprite = Resources.Load<Sprite>("Letters/I"); break;
            case 'J': mySprite = Resources.Load<Sprite>("Letters/J"); break;
            case 'K': mySprite = Resources.Load<Sprite>("Letters/K"); break;
            case 'L': mySprite = Resources.Load<Sprite>("Letters/L"); break;
            case 'M': mySprite = Resources.Load<Sprite>("Letters/M"); break;
            case 'N': mySprite = Resources.Load<Sprite>("Letters/N"); break;
            case 'O': mySprite = Resources.Load<Sprite>("Letters/O"); break;
            case 'P': mySprite = Resources.Load<Sprite>("Letters/P"); break;
            case 'Q': mySprite = Resources.Load<Sprite>("Letters/Q"); break;
            case 'R': mySprite = Resources.Load<Sprite>("Letters/R"); break;
            case 'S': mySprite = Resources.Load<Sprite>("Letters/S"); break;
            case 'T': mySprite = Resources.Load<Sprite>("Letters/T"); break;
            case 'U': mySprite = Resources.Load<Sprite>("Letters/U"); break;
            case 'V': mySprite = Resources.Load<Sprite>("Letters/V"); break;
            case 'W': mySprite = Resources.Load<Sprite>("Letters/W"); break;
            case 'X': mySprite = Resources.Load<Sprite>("Letters/X"); break;
            case 'Y': mySprite = Resources.Load<Sprite>("Letters/Y"); break;
            case 'Z': mySprite = Resources.Load<Sprite>("Letters/Z"); break;
            default:
                if (mySprite == null)
                    throw new System.Exception("Cannot find valid sprite.");
                break;
        }

        GameObject myGameObject = new GameObject("letter " + letter.ToString());
        Image image = myGameObject.AddComponent<Image>();
        image.sprite = mySprite;

        return myGameObject;
    }

}
