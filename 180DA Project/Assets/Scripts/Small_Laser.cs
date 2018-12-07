using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Small_Laser : MonoBehaviour {

	public AudioClip smallLaserSound;
	void Start () {
		GameState.PlayClip(smallLaserSound);
	}
	
	
	void Update () {
		
	}
}
