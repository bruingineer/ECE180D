using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Small_Laser_Obstacle_Minigame : Small_Laser_Obstacle {
	protected override void PrefabSetup()
	{
		laserPrefab = (Resources.Load("Prefabs/Lasers/Small_Laser") as GameObject).AddComponent<Small_Laser_Minigame>();
	}
}
