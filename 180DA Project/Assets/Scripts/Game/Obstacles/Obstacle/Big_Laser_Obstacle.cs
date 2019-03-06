using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Big_Laser_Obstacle : Laser_Obstacle {

	// Sound strings to load from
    private string countdownEasy = "Easy";
    public string countdownMedium = "Medium";
    private string countdownHard = "Hard";
	private string soundsPath = "Sounds/Laser/";
	private string lasersPath = "Prefabs/Lasers/";
	
	// Variables
	private string laserCountdown_str;
	private string laserSound = "Big_Laser_Sound";
	private AudioClip laserFire;
	private AudioClip laserCountdown;

	// Time Parameters
	private float cooldown = 0.75f;

	// Extra Parameters
	private int minLasers;
	private int maxLasers;
	
	void Awake () {
        // Choose # of lasers to fire based on difficulty selected
		// Choose countdown time based on difficulty
        if (SelectedPlayer.current_difficulty == "easy") 
		{ 
			laserCountdown_str = countdownEasy;
			minLasers = 4;
			maxLasers = 7;
		}
        else if (SelectedPlayer.current_difficulty == "medium") 
		{
			laserCountdown_str = countdownMedium;
			minLasers = 5;
			maxLasers = 8;
		}
        else if (SelectedPlayer.current_difficulty == "hard") 
		{
			laserCountdown_str = countdownHard;
			minLasers = 6;
			maxLasers = 9;
		}
		// Load sounds from Resources
		laserCountdown = Resources.Load<AudioClip>(soundsPath + laserCountdown_str + "_Big_Laser");
		laserFire = Resources.Load<AudioClip>(soundsPath + laserSound);
		// how long the lasers take to get to desired location
		laserDuration = 0.005f;
		// Adding times for lasers
		laserTimes = new Laser_Times(laserDuration, cooldown);
		// Load Prefab
		laserPrefab = (Resources.Load(lasersPath + "BigLaser") as GameObject);
	}

	protected override IEnumerator FireLasers(List<int> laserPositions) 
	{ 
		List<GameObject> lasers = CreateLasers(laserPositions);

		GameState_Base.PlayClip(laserCountdown);
		// Instantiate warnings at the lanes acquired
		CreateWarnings(lasers, laserCountdown.length);
		// waits the length of the warmup track
		yield return new WaitForSeconds(laserCountdown.length);
		// plays audio to fire lasers
		GameState_Base.PlayClip(laserFire);
		// creates lasers at locations and then moves them
		MoveLasers(lasers);
		// waits for cooldown until a new obstacle can turn on
		yield return new WaitForSeconds(obstacleWaitTime + cooldown);
	}

	protected override List<int> RandomizeLanes()
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

	// add laser prefab component to laser later
	protected List<GameObject> CreateLasers(List<int> laserPositions)
	{
		List<GameObject> lasers = new List<GameObject>();
		int list_count = laserPositions.Count;
		for(int i = 0; i < list_count; i++)
		{
			// create laser prefab
			GameObject prefab = Instantiate(laserPrefab, new Vector3(laserPositions[i] + 0.5f, GameState_Base.end_row), Quaternion.Euler(0, 0, 90)) as GameObject;
			// add component based on whether it is a Minigame or maingame
			AddComponentToLaser(prefab);
			lasers.Add(prefab);
		}
		return lasers;
	}



	// Play the laser warning on the laser prefab
	private void CreateWarnings(List<GameObject> lasers, float laserWarmUpTime)
	{
		int laser_count = lasers.Count;
		for(int i = 0; i < laser_count; i++)
		{
			ParticleSystem ps = lasers[i].GetComponent<ParticleSystem>();
			var main = ps.main;
			main.duration = laserWarmUpTime;
			ps.Play();
		}
	}

	// Move laser to other side of the screen
	private void MoveLasers(List<GameObject> lasers)
	{
		int laser_count = lasers.Count;
		for (int i = 0; i < laser_count; i++)
		{
			if(lasers[i])
				lasers[i].GetComponent<Big_Laser>().MoveLaser(new Vector3(lasers[i].transform.position.x, 0), laserTimes);
		}
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
