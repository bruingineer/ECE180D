using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Big_Laser : Laser {

	// Objects 
	private LineRenderer lineRenderer;
	// States
	// playerHit is static 
	static bool playerHit = false;

	void Awake () {
		// grabs the line render component from the beam child
		lineRenderer = transform.Find("Beam").GetComponent<LineRenderer>();
		// sets the initial position to wherever the beam was created 
		lineRenderer.SetPosition(0, transform.position);
	}
	
	void Update () {
		Play();
		CheckGamePlaying();
	}

	void Play()
	{
		// RaycastHit returns information regarding the hit from the LineRenderer (you can access it's transform)
		RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.left, out hit) && !playerHit)
		{
			playerHit = true;
            SelectedPlayer.current_hits += 1;
			Laser_MiniGame.playerWasHit = true;
			Player m_player = hit.transform.GetComponent<Player>();
			if (m_player)
				m_player.PlayerHit();
		}
	}

	protected override IEnumerator MoveLaserToPosition(Vector3 endPosition, Laser_Obstacle.Laser_Times laserTimes)
   	{
		var initialPos = transform.position;
		float t = 0f;
		while(t < 1)
		{
				t += Time.deltaTime / laserTimes.duration;
				lineRenderer.SetPosition(1, Vector3.Lerp(initialPos, endPosition, t));
				yield return null;
		}
		yield return null;
		Destroy(gameObject, laserTimes.cooldown);
    }
}
