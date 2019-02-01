using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmallLaser : Laser {
	// Sounds
	public AudioClip smallLaserSound;
	// Objects to interact with 
	private GameObject player;
	void Start () {
		player = GameObject.FindGameObjectWithTag("Player");
		GameState.PlayClip(smallLaserSound);
    }
	
	void Update()
	{
		// CheckGamePlaying();
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
		Player m_player = other.GetComponent<Player>();
		if (m_player)
			m_player.PlayerHit();
		// used for the minigame, will be taken out when a small laser mini game mode is created 
		Laser_MiniGame.playerWasHit = true;
		Destroy(gameObject);
	}
}
