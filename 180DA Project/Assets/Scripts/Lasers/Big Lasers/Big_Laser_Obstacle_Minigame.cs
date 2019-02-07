using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Big_Laser_Obstacle_Minigame : Big_Laser_Obstacle {
	
	protected override void PrefabSetup()
	{
		laserPrefab = (Resources.Load("Prefabs/Lasers/SmallLaser") as GameObject).AddComponent<Big_Laser_Minigame>();
	}
}
