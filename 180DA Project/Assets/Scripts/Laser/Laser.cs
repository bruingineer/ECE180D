using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : Moving_Object {
	// function from the class that actually calls the coroutine
	protected virtual IEnumerator MoveLaserToPosition(Vector3 endPosition, Laser_Obstacle.Laser_Times laserTimes) {yield return null;}
	

	// called from the obstacle mini game to move the laser
	public void MoveLaser(Vector3 endPosition, Laser_Obstacle.Laser_Times laserTimes)
   	{
		StartCoroutine(MoveLaserToPosition(endPosition, laserTimes));
    }
	// checks to see if the game is still playing, if not, the object is destroyed
	protected void CheckGamePlaying()
	{
		if (!GameState.gamePlaying)
			Destroy(gameObject);
	}
}
