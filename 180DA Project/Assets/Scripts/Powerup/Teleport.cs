using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleport : MonoBehaviour {

	static public bool On = false;

	public AudioClip sound;

	void Awake(){
		sound = Resources.Load<AudioClip>("Sounds/Laser/Small_Laser_Sound");
	}

	private void OnTriggerEnter(Collider other)
	{
		if (this.gameObject.tag == "from" && other.gameObject.tag == "Player"){
			other.transform.position = Powerup.tele_to;
			if (other.transform.position != Powerup.tele_to)
				Debug.Log("Not the same");
			Destroy(this.gameObject);
		}

		if (this.gameObject.tag == "to" && other.gameObject.tag == "Player"){
			Destroy(this.gameObject);
			On = false;
		}
	}

	// IEnumerator Teleport_to(ref Collider other){
	// 	other.transform.position = Powerup.tele_to;
	// 	if (other.transform.position != Powerup.tele_to)
	// 		Debug.Log("Not the same");
	// 	Destroy(this.gameObject);
	// 	return null;
	// }

}
