using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser_Obstacle : MonoBehaviour {
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

	protected Laser_Times laserTimes;
}
