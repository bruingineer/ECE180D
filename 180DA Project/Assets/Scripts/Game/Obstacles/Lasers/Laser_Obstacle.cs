using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This base class is used for both the Small Laser and Big Laser classes
public abstract class Laser_Obstacle : MonoBehaviour {
	
	// This represents the gambeobject used by the laser obstacle
	protected GameObject laserPrefab;
	// object representing the times used by the laser obstacle
	protected Laser_Times laserTimes;
	// how long the laser is shown for
	protected float laserDuration;

	protected float obstacleWaitTime = 3f;

	// struct is used to pass in the correct times to the obstacle times
	public struct Laser_Times
	{
    	public float duration, cooldown;

    	public Laser_Times(float dur, float cool = 0f)
    	{
        	duration = dur;
        	cooldown = cool;
    	}
	}

	// starts the laser obstacle
	public IEnumerator StartObstacle()
	{
		yield return FireLasers();
	}

	// function to be implemented by laser obstacle that fires the lasers
	protected abstract IEnumerator FireLasers();
}
