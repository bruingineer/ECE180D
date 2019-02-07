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
		player = GameObject.Find("Player").GetComponent<Player>();
		laserTimes = new Laser_Times(laserDuration, waitForNextLaser);
	}
	
	protected virtual void PrefabSetup()
	{
		laserPrefab = (Resources.Load("Prefabs/Lasers/Small_Laser") as GameObject).AddComponent<Small_Laser>();
	}

	protected override IEnumerator FireLasers() 
	{
		// get initial and final x position
		bool switchPosition = !(player.transform.position.x < GameState.end_column / 2);
		float start_X_Position = switchPosition ? 0 : GameState.end_column;
		float end_X_Position = switchPosition ? GameState.end_column : 0;
		// Create lasers
		yield return CreateLasers(start_X_Position, end_X_Position);
		// wait until last laser ends plus waiting for the next obstacle time
		yield return new WaitForSeconds(laserTimes.duration + obstacleWaitTime);
		
	}

	private IEnumerator CreateLasers(float start_X_Position, float end_X_Position)
	{
		for(int i = 0; i < lasersToFire; i++) 
		{
			Small_Laser small_laser = Instantiate(laserPrefab, new Vector3(start_X_Position, 
				GameState.laneNums[Random.Range(0, GameState.laneNums.Count)] + 0.5f), 
					Quaternion.identity) as Small_Laser;
			small_laser.MoveLaser(new Vector3(end_X_Position, small_laser.transform.position.y), laserTimes);
			yield return new WaitForSeconds(waitForNextLaser);
		}
		yield return null;
	}
}
