using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Big_Laser_Obstacle : Laser_Obstacle {

	// Sounds
    public AudioClip countdownEasy;
    public AudioClip countdownMedium;
    public AudioClip countdownHard;
	// use this to assign to the others TODO
	public AudioClip laserCountdown;
	public AudioClip laserFire;

	// Time Parameters
	private float cooldown = 0.75f;

	// Objects
	public GameObject laserWarning;
	private List<GameObject> laserWarnings;

	// Extra Parameters
	// change these parameters based on difficulty TODO
	private int minLasers = 6;
	private int maxLasers = 9;
	
	void Awake () {
        //Choose # of lasers to fire based on difficulty selected
        if (SelectedPlayer.current_difficulty == "easy") laserCountdown = countdownEasy;
        else if (SelectedPlayer.current_difficulty == "medium") laserCountdown = countdownMedium;
        else if (SelectedPlayer.current_difficulty == "hard") laserCountdown = countdownHard;

		laserDuration = 0.005f;
		// Adding times for lasers
		laserTimes = new Laser_Times(laserDuration, cooldown);
        laserWarnings = new List<GameObject>();
		// reset playerHit for big laser objects
	}

	void Update()
	{
		// CheckGamePlaying();
	}
	protected override IEnumerator FireLasers() 
	{
		List<int> lanesToFireLasers = GetRandomLanes();
		// Play warm up clip
		GameState.PlayClip(laserCountdown);
		// Instantiate warnings at the lanes acquired
		CreateWarnings(lanesToFireLasers, laserCountdown.length);
		// waits the length of the warmup track
		yield return new WaitForSeconds(laserCountdown.length);
		// plays audio to fire lasers
		GameState.PlayClip(laserFire);
		// creates lasers at locations and then moves them
		CreateLasers(lanesToFireLasers);
		// destroys all laser warnings
		while(laserWarnings.Count > 0) {
			Destroy(laserWarnings[0]);
			laserWarnings.RemoveAt(0);
		}
		// waits for cooldown until a new obstacle can turn on
		yield return new WaitForSeconds(obstacleWaitTime + cooldown);
		Destroy(gameObject);
	}

	protected void CreateLasers(List<int> lanesToFireLasers)
	{
		Big_Laser.playerHit = false;
		int list_count = lanesToFireLasers.Count;
		for(int i = 0; i < list_count; i++)
		{
			Laser bigLaser = Instantiate(laserPrefab, new Vector3(GameState.end_column, (lanesToFireLasers[i] + 0.5f)), Quaternion.identity) as Laser;
			bigLaser.GetComponent<Big_Laser>().MoveLaser(new Vector3(0, bigLaser.transform.position.y), laserTimes);
		}
	}

	private void CreateWarnings(List<int> lanesToFireLasers, float laserWarmUpTime)
	{
		int list_count = lanesToFireLasers.Count;
		for(int i = 0; i < list_count; i++)
		{
			GameObject laserParticle = Instantiate(laserWarning, new Vector3(GameState.end_column, (lanesToFireLasers[i] + 0.5f)), Quaternion.identity);
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
