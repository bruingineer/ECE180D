using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser_Obstacle : Moving_Object {
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
	public GameObject laserPrefab;
	protected Laser_Times laserTimes;
	protected float laserDuration;

	protected virtual IEnumerator FireLasers() {yield return null;}
}
