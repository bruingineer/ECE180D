using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Small_Laser : Laser {

	public AudioClip smallLaserSound;
	private GameObject player;
	void Start () {
		player = GameObject.FindGameObjectWithTag("Player");
		GameState.PlayClip(smallLaserSound);
    }
	
	void Update()
	{
		if (!GameState.gamePlaying)
			Destroy(gameObject);
	}

	public void MoveLaser(float end_x_position, float timeToMove)
   	{
		StartCoroutine(MoveLaserToPosition(end_x_position, timeToMove));
    }

	public IEnumerator MoveLaserToPosition(float end_x_position, float timeToMove)
	{
		yield return MoveToPosition(new Vector3(end_x_position, transform.position.y), timeToMove);
		Destroy(gameObject);
		yield return null;
	}
	private void OnTriggerEnter(Collider other)
	{
		Player m_player = other.GetComponent<Player>();
		if (m_player)
			m_player.isHit = true;
		Laser_MiniGame.playerWasHit = true;
		Destroy(gameObject);
	}
}
