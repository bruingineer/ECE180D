using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Big_Laser : Laser {

	// Objects 
	private LineRenderer lineRenderer;
	private bool laserStart = false;
	// States
	static public bool playerHit;

	void Awake () {
		// grabs the line render component from the beam child
		lineRenderer = transform.GetComponent<LineRenderer>();
		// sets the initial position to wherever the beam was created 
		lineRenderer.SetPosition(0, transform.position);
		playerHit = false;
	}
	
	void Update () {
		CheckHitPlayer();
	}

	void CheckHitPlayer()
	{
		// RaycastHit returns information regarding the hit from the LineRenderer (you can access it's transform)
		RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit) && !playerHit && laserStart)
		{
			playerHit = true;
			PlayerHit(hit.transform);
		}
	}

	protected override IEnumerator MoveLaserToPosition(Vector3 endPosition, Laser_Obstacle.Laser_Times laserTimes)
   	{
		laserStart = true;
		lineRenderer.enabled = true;
		var initialPos = transform.position;
		float t = 0f;
		while(t < 1)
		{
				t += Time.deltaTime / laserTimes.duration;
				lineRenderer.SetPosition(1, Vector3.Lerp(initialPos, endPosition, t));
				yield return null;
		}
		Destroy(gameObject, laserTimes.cooldown);
		yield return null;
    }
}

public class Big_Laser_Minigame : Big_Laser {
	protected override void Handle_Minigame()
	{
		Obstacles_Laser_Minigame.laserHit = true;
	}
}