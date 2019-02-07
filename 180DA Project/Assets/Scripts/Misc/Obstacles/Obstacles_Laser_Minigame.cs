﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacles_Laser_Minigame : Obstacles {

	public bool laserHit = false;
	private Player m_player;

	protected override void Awake()
	{
		m_player = GameObject.Find("Player").GetComponent<Player>();
		bigLaserObstacle = gameObject.AddComponent<Big_Laser_Obstacle_Minigame>();
		smallLaserObstacle = gameObject.AddComponent<Small_Laser_Obstacle_Minigame>();
		obstacles = new List<Obstacle> {bigLaserObstacle, smallLaserObstacle};
	}

	protected override void Handle_Minigame()
	{
		if (laserHit)
			m_player.MovePlayer();
		
		laserHit = false;
	}
}