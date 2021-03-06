﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacles_Laser_Minigame : Obstacles_Main {

	public static bool laserHit;
	private Player m_player;

	protected override void Awake()
	{
		m_player = GameObject.FindWithTag("Player").GetComponent<Player_Main>();
		bigLaserObstacle = gameObject.AddComponent<Big_Laser_Obstacle_Minigame>();
		smallLaserObstacle = gameObject.AddComponent<Small_Laser_Obstacle_Minigame>();
		obstacles = new List<Laser_Obstacle> {bigLaserObstacle, smallLaserObstacle};
		laserHit = false;
	}

	protected override void Handle_Minigame()
	{
		if (!laserHit && GameState_Base.gamePlaying)
			m_player.MovePlayer();
		laserHit = false;
	}
}
