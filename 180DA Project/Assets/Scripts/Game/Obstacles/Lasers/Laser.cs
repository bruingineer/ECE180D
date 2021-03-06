﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// base class fo the actual laser object
public abstract class Laser : Moving_Object {
	// function from the derived laser class that actually calls the coroutine
	protected virtual IEnumerator MoveLaserToPosition(Vector3 endPosition, Laser_Obstacle.Laser_Times laserTimes) {yield return null;}

	// Gives the obstacles a function to call the lasers
	public void MoveLaser(Vector3 endPosition, Laser_Obstacle.Laser_Times laserTimes)
	{
		StartCoroutine(MoveLaserToPosition(endPosition, laserTimes));
	}

	protected virtual void Handle_Minigame() {}

	protected void PlayerHit(Transform hit)
	{
		Player m_player;
		m_player = hit.GetComponent<Player>();
		if (m_player)
			{
				m_player.PlayerHit();
				Handle_Minigame();
			}
	}
	
}
