using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Laser_Obstacle : Obstacle {
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
	public override IEnumerator StartObstacle()
	{
		yield return FireLasers();
	}

	protected IEnumerator WaitandFreeObstacle(float timeToWait)
	{
		yield return new WaitForSeconds(timeToWait);
	}

	protected GameObject laserPrefab;
	protected Laser_Times laserTimes;
	protected float laserDuration;
	protected abstract IEnumerator FireLasers();
}
