using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Big_Laser_Obstacle_Minigame : Big_Laser_Obstacle {
	public bool laserHit = false;
	protected override void Handle_MiniGame() 
	{
		if (!laserHit)
			GameObject.Find("Player").GetComponent<Player>().MovePlayer();
		else
			laserHit = true;
	}	
}
