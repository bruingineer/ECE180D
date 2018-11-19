using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameState : MonoBehaviour {

	public int numLanes;
	public static int middle_lane;
	
	void Start () {
		// change to make it dynamic
		numLanes = 10;
		middle_lane = numLanes/2;
	}
	
	// // Update is called once per frame
	// void Update () {
		
	// }
}
