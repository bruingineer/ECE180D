using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Powerup : MonoBehaviour {

	public static int powerup_count = 0;
	// Use this for initialization

	static public bool superOn = false;

	void Start(){
		start = new Vector3(GameState_with_Player.numLanes / 2, 0.5f);
		teleportOn = false;
		powerup_count = 0;
	}


	public GameObject from;
	public GameObject to;
	static public bool teleportOn;
	// Update is called once per frame
	static bool resetCount = false;
	bool cooldown = false;

	IEnumerator CoolDownTimer(){
		yield return new WaitForSeconds(20);
		cooldown = false;
	}

	void Update () {
		if (powerup_count > 4 && !cooldown){
			int x = UnityEngine.Random.Range(0,1);
			if (x == 0){
				Debug.Log("Give Player Teleport");
				powerup_count = 0;
				AddTeleport();
				teleportOn = true;
				cooldown = true;
				StartCoroutine(CoolDownTimer());
			}
			else{
				powerup_count  = 0;
				superOn = true;
				Debug.Log("Give player Super");
				GiveSuper();
				cooldown = true;
				StartCoroutine(CoolDownTimer());
			}
	
		}

	}

	public void GiveSuper(){
		GameObject.FindWithTag("Player").GetComponent<Player>().superActive = true;
	}

 	Vector3 start;
	static public Vector3 tele_from;
	static public Vector3 tele_to;
	public GameObject Super;
	public void AddTeleport(){
		int row = UnityEngine.Random.Range(0,10);
		Vector3 player_pos = GameObject.FindWithTag("Player").transform.position;
		if (player_pos[1] > 11 - 4){
			GiveSuper();
			return;
		}
		else{
			tele_from = new Vector3((float)row , player_pos[1] + 2f);
			tele_to = new Vector3((float)row , player_pos[1] + 4f);
		}


		from = (Resources.Load("Prefabs/Powerups/From") as GameObject);

		to = (Resources.Load("Prefabs/Powerups/To") as GameObject);

		from = Instantiate(from, tele_from, Quaternion.identity);
		to = Instantiate(to, tele_to, Quaternion.identity);
	}

}
