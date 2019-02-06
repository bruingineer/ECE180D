using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Small_Laser_Minigame : Small_Laser {

	protected override void Handle_Minigame()
	{
		GetComponent<Obstacles_Laser_Minigame>().laserHit = true;
	}
}
