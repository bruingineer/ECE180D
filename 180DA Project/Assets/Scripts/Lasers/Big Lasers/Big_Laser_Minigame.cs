using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Big_Laser_Minigame : Big_Laser {
	protected override void Handle_Minigame()
	{
		GetComponent<Big_Laser_Obstacle_Minigame>().laserHit = true;
	}
}
