using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleport : MonoBehaviour {

	private bool On = false;

	public AudioClip sound;

	void Awake(){
		sound = Resources.Load<AudioClip>("Sounds/Laser/Small_Laser_Sound");
	}

	
	void Update (){
		Vector3 player_pos = GameObject.FindGameObjectWithTag("Player").transform.position;
		if (player_pos[1] > Powerup.tele_from[1]){
			Destroy(this.gameObject);
			Powerup.teleportOn = false;
		}

	}

	private void OnTriggerEnter(Collider other)
	{
		if (this.gameObject.tag == "from" && other.gameObject.tag == "Player"){
			other.GetComponent<Player>().teleportActve = true;
			Debug.Log("teleporting from");
			Destroy(this.gameObject);
			On = true;
		}

		if (this.gameObject.tag == "to" && other.gameObject.tag == "Player"){
			Destroy(this.gameObject);
		}
	}

}
