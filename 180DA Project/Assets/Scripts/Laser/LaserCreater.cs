﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserCreater : MonoBehaviour {
    public AudioClip countdownEasy;
    public AudioClip countdownMedium;
    public AudioClip countdownHard;
	public AudioClip laserCountdown;

	public AudioClip laserFire;
	public GameObject laserPrefab;
	private int minLasers = 6;
	private int maxLasers = 9;
	public GameObject laserWarning;
	private List<GameObject> laserWarnings;

	void Start () {

        //Choose # of lasers to fire based on difficulty selected
        if (SelectedPlayer.current_difficulty == "easy") laserCountdown = countdownEasy;
        else if (SelectedPlayer.current_difficulty == "medium") laserCountdown = countdownMedium;
        else if (SelectedPlayer.current_difficulty == "hard") laserCountdown = countdownHard;

        laserWarnings = new List<GameObject>();
		StartCoroutine(ShootLasers());
	}

	void Update()
	{
		if (!GameState.gamePlaying)
			Destroy(gameObject);
	}
	private IEnumerator ShootLasers() 
	{
		List<int> lanesToFireLasers = GetRandomLanes();
		int list_count = lanesToFireLasers.Count;
		GameState.PlayClip(laserCountdown);
		float laserWarmUpTime = laserCountdown.length;
		for(int i = 0; i < list_count; i++)
		{
			GameObject laserParticle = Instantiate(laserWarning, new Vector3(GameState.end_column, (lanesToFireLasers[i] + 0.5f)), Quaternion.identity);
			ParticleSystem ps = laserParticle.GetComponent<ParticleSystem>();
			var main = ps.main;
			main.duration = laserWarmUpTime;
			ps.Play();
			laserWarnings.Add(laserParticle);
		}
		yield return new WaitForSeconds(laserCountdown.length);
		GameState.PlayClip(laserFire);
		for(int i = 0; i < list_count; i++)
		{
			Instantiate(laserPrefab, new Vector3(GameState.end_column, (lanesToFireLasers[i] + 0.5f)), Quaternion.identity);
		}
		while(laserWarnings.Count > 0) {
			Destroy(laserWarnings[0]);
			laserWarnings.RemoveAt(0);
		}
		
		yield return new WaitForSeconds(Obstacles.obstacleWaitTime + Laser.destroyLaserDelay);
		Obstacles.obstacleOn = false;
		Laser_MiniGame.obstacleOn = false;
		Destroy(gameObject);
	}
	private List<int> GetRandomLanes() 
	{
		List<int> laneNumsCopy = new List<int>(GameState.laneNums);
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
}
