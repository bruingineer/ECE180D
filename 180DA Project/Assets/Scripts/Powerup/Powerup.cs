using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Powerup : MonoBehaviour {

	public static int powerup_count = 0;
	// Use this for initialization

	static public bool superOn = false;

	void Start(){
		start = new Vector3(GameState_Base.numLanes / 2 - 0.5f, 0.5f);
		teleportOn = false;
		powerup_count = 0;
	}


	public GameObject from;
	public GameObject to;
	bool teleportOn;
	// Update is called once per frame
	static bool resetCount = false;
	void Update () {
		if(powerup_count == 2 && !teleportOn){
			Debug.Log("Give Player Teleport");
			powerup_count = 0;
			AddTeleport();
			teleportOn = true;
		}

		if (powerup_count == 3){
			superOn = true;
			Debug.Log("Give player Super");
			
		}
	}

	Vector3 start;
	
	static public Vector3 tele_from;
	static public Vector3 tele_to;
	public GameObject Super;
	public void AddTeleport(){
		Vector3 player_pos = GameObject.FindWithTag("Player").transform.position;
		tele_from = new Vector3(1.5f, player_pos[1] + 2f);
		tele_to = new Vector3(6.5f, player_pos[1] + 5f);


		from = (Resources.Load("Prefabs/Powerups/From") as GameObject);

		to = (Resources.Load("Prefabs/Powerups/To") as GameObject);

		from = Instantiate(from, tele_from, Quaternion.identity);
		to = Instantiate(to, tele_to, Quaternion.identity);
	}

}
