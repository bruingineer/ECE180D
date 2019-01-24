using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Small_Laser : MonoBehaviour {

	public AudioClip smallLaserSound;
	public static float laserTime = 4f;
	private GameObject player;
	public float end_position;
	void Start () {
		player = GameObject.FindGameObjectWithTag("Player");
		GameState.PlayClip(smallLaserSound);
		StartCoroutine(MoveToPosition(new Vector3(end_position, transform.position.y), laserTime));
	}
	
	void Update()
	{
		if (!GameState.gamePlaying)
			Destroy(gameObject);
	}

	public IEnumerator MoveToPosition(Vector3 end_position, float timeToMove)
   	{
		var initialPos = transform.position;
		float t = 0f;
		while(t < 1)
		{
				t += Time.deltaTime / timeToMove;
				transform.position = Vector3.Lerp(initialPos, end_position, t);
				yield return null;
		}
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
