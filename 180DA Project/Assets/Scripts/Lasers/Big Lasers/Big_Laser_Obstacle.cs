﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Big_Laser_Obstacle : Laser_Obstacle {

	// Sounds
    private string countdownEasy = "Easy";
    public string countdownMedium = "Medium";
    private string countdownHard = "Hard";
	private string soundsPath = "Sounds/";
	private string lasersPath = "Prefabs/Lasers/";
	// use this to assign to the others TODO
	private string laserCountdown_str;
	private string laserSound = "Big_Laser_Sound";
	private AudioClip laserFire;
	private AudioClip laserCountdown;

	// Time Parameters
	private float cooldown = 0.75f;

	// Objects
	private GameObject laserWarning;
	private List<GameObject> laserWarnings;

	// Extra Parameters
	// change these parameters based on difficulty TODO
	private int minLasers = 6;
	private int maxLasers = 9;
	
	void Awake () {
        //Choose # of lasers to fire based on difficulty selected
        if (SelectedPlayer.current_difficulty == "easy") laserCountdown_str = countdownEasy;
        else if (SelectedPlayer.current_difficulty == "medium") laserCountdown_str = countdownMedium;
        else if (SelectedPlayer.current_difficulty == "hard") laserCountdown_str = countdownHard;
		laserCountdown = Resources.Load<AudioClip>(soundsPath + laserCountdown_str + "_Big_Laser");
		laserFire = Resources.Load<AudioClip>(soundsPath + laserSound);
		laserDuration = 0.005f;
		// Adding times for lasers
		laserTimes = new Laser_Times(laserDuration, cooldown);
        laserWarnings = new List<GameObject>();
		// Finish loading sounds
		laserWarning = (Resources.Load("Prefabs/Lasers/Laser_Warning") as GameObject);
		laserPrefab = (Resources.Load(lasersPath + "BigLaser") as GameObject);
	}

	void Update()
	{
		// CheckGamePlaying();
	}

	protected override IEnumerator FireLasers() 
	{
		List<int> lanesToFireLasers = GetRandomLanes();
		// Play warm up clip
		GameState_Base.PlayClip(laserCountdown);
		// Instantiate warnings at the lanes acquired
		CreateWarnings(lanesToFireLasers, laserCountdown.length);
		// waits the length of the warmup track
		yield return new WaitForSeconds(laserCountdown.length);
		// plays audio to fire lasers
		GameState_Base.PlayClip(laserFire);
		// creates lasers at locations and then moves them
		CreateLasers(lanesToFireLasers);
		// destroys all laser warnings
		while(laserWarnings.Count > 0) {
			Destroy(laserWarnings[0]);
			laserWarnings.RemoveAt(0);
		}
		// waits for cooldown until a new obstacle can turn on
		yield return new WaitForSeconds(obstacleWaitTime + cooldown);
	}

	protected virtual void Handle_MiniGame() {}

	// add laser prefab component to laser later
	protected void CreateLasers(List<int> lanesToFireLasers)
	{
		Big_Laser.playerHit = false;
		int list_count = lanesToFireLasers.Count;
		for(int i = 0; i < list_count; i++)
		{
			GameObject prefab = Instantiate(laserPrefab, new Vector3(lanesToFireLasers[i] + 0.5f, GameState_Base.end_row), Quaternion.identity) as GameObject;
			Big_Laser bigLaser = AddComponentToLaser(prefab).GetComponent<Big_Laser>();
			bigLaser.MoveLaser(new Vector3(bigLaser.transform.position.x, 0), laserTimes);
		}
	}

	private void CreateWarnings(List<int> lanesToFireLasers, float laserWarmUpTime)
	{
		int list_count = lanesToFireLasers.Count;
		for(int i = 0; i < list_count; i++)
		{
			GameObject laserParticle = Instantiate(laserWarning, new Vector3(lanesToFireLasers[i] + 0.5f, GameState_Base.end_row), Quaternion.Euler(0, 0, 90));
			ParticleSystem ps = laserParticle.GetComponent<ParticleSystem>();
			var main = ps.main;
			main.duration = laserWarmUpTime;
			ps.Play();
			laserWarnings.Add(laserParticle);
		}
	}

	// get lanes to fire the lasers
	private List<int> GetRandomLanes() 
	{
		List<int> laneNumsCopy = new List<int>(GameState_Base.laneNums);
		List<int> lanesToFireLasers = new List<int>();
		int numLasersToFire = Random.Range(minLasers, maxLasers);
		for (int i = 0; i < numLasersToFire; i++)
		{
			int index = Random.Range(0, laneNumsCopy.Count);
			lanesToFireLasers.Add(laneNumsCopy[index]);
			laneNumsCopy.RemoveAt(index);
		}
		return lanesToFireLasers;
	}

	protected virtual GameObject AddComponentToLaser(GameObject prefab)
	{
		prefab.AddComponent<Big_Laser>();
		return prefab;
	}
}

public class Big_Laser_Obstacle_Minigame : Big_Laser_Obstacle {
	
	protected override GameObject AddComponentToLaser(GameObject prefab)
	{
		prefab.AddComponent<Big_Laser_Minigame>();
		return prefab;
	}
}
