using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Powerup : MonoBehaviour {

	public static int powerup_count = 0;
	// Use this for initialization
	
	// Update is called once per frame
	void Update () {
		if(powerup_count == 4){
			Debug.Log("Give Player Powerup");
		}
	}



}
