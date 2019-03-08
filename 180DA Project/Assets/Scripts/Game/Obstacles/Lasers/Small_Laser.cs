using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Small_Laser : Laser {
	// Sounds
	public AudioClip smallLaserSound;
	// Objects to interact with 
	private GameObject player;
	void Awake () {
		player = GameObject.FindGameObjectWithTag("Player");
		smallLaserSound = Resources.Load<AudioClip>("Sounds/Laser/Small_Laser_Sound");
		GameState_Base.PlayClip(smallLaserSound);
  }

	void Update()
	{
			CheckGamePlaying();
	}


	protected override IEnumerator MoveLaserToPosition(Vector3 endPosition, Laser_Obstacle.Laser_Times laserTimes)
	{
		// Moves position then destroys object when coroutine finishes
		yield return MoveToPosition(endPosition, laserTimes.duration);
		Destroy(gameObject);
		yield return null;
	}
	
	// Gets triggered when laser enters the player's collider
	private void OnTriggerEnter(Collider other)
	{
		// gets component of the object, if player, it will return true, and the player will get hit
		if (other.gameObject.tag == "Player"){
			PlayerHit(other.transform);
			Destroy(gameObject);
		}
	}

}

public class Small_Laser_Minigame : Small_Laser {

	protected override void Handle_Minigame()
	{
		Obstacles_Laser_Minigame.laserHit = true;
	}
}
