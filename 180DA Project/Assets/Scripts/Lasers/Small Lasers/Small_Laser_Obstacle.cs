using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Small_Laser_Obstacle : Laser_Obstacle {

	// Objects
	private Player player;

	// Parameters
	// changed based on difficulty in the future
	private int lasersToFire;
	private float waitForNextLaser;

	void Update()
	{
		// CheckGamePlaying();
	}
	
	void Awake()
	{
		//Choose # of lasers to fire and laser speed based on difficulty selected
        if (SelectedPlayer.current_difficulty == "easy") 
		{
			laserDuration = 4f;
			lasersToFire = 10;
			waitForNextLaser = .5f;
		}
        else if (SelectedPlayer.current_difficulty == "medium") {
			laserDuration = 3f;
			lasersToFire = 15;
			waitForNextLaser = 0.4f;
		}
        else if (SelectedPlayer.current_difficulty == "hard") {
			laserDuration = 2.5f;
			lasersToFire = 20;
			waitForNextLaser = 0.3f;
		}
		PrefabSetup();
		player = GameObject.FindWithTag("Player").GetComponent<Player>();
		laserTimes = new Laser_Times(laserDuration, waitForNextLaser);
	}
	
	protected virtual void PrefabSetup()
	{
		laserPrefab = (Resources.Load("Prefabs/Lasers/Small_Laser") as GameObject).AddComponent<Small_Laser>();
	}

	protected override IEnumerator FireLasers() 
	{
		// get initial and final x position
		bool switchPosition = !(player.transform.position.y < GameState_Base.end_row / 2);
		float start_Y_Position = switchPosition ? 0 : GameState_Base.end_row;
		float end_Y_Position = switchPosition ? GameState_Base.end_row : 0;
		// Create lasers
		yield return CreateLasers(start_Y_Position, end_Y_Position);
		// wait until last laser ends plus waiting for the next obstacle time
		yield return new WaitForSeconds(laserTimes.duration + obstacleWaitTime);
		
	}

	private IEnumerator CreateLasers(float start_Y_Position, float end_Y_Position)
	{
		for(int i = 0; i < lasersToFire; i++) 
		{
			Small_Laser small_laser = Instantiate(laserPrefab, new Vector3(GameState_Base.laneNums[Random.Range(0, GameState_Base.laneNums.Count)] + 0.5f, 
					start_Y_Position), Quaternion.Euler(0, 0, 90)) as Small_Laser;
			small_laser.MoveLaser(new Vector3(small_laser.transform.position.x, end_Y_Position), laserTimes);
			yield return new WaitForSeconds(waitForNextLaser);
		}
		yield return null;
	}
}

public class Small_Laser_Obstacle_Minigame : Small_Laser_Obstacle {
	protected override void PrefabSetup()
	{
		laserPrefab = (Resources.Load("Prefabs/Lasers/Small_Laser") as GameObject).AddComponent<Small_Laser_Minigame>();
	}
}