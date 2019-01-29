using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Small_Laser_Obstacle : Laser_Obstacle {

	public GameObject smallLaser;
	private int lasersToFire;
	private float waitForNextLaser;
	private GameObject player;
	private float laserDuration;

	void Update()
	{
		if (!GameState.gamePlaying)
			Destroy(gameObject);
	}
	
	void Start () {
		
		//Choose # of lasers to fire based on difficulty selected
        if (SelectedPlayer.current_difficulty == "easy") laserDuration = 4f;
        else if (SelectedPlayer.current_difficulty == "medium") laserDuration = 3f;
        else if (SelectedPlayer.current_difficulty == "hard") laserDuration = 2.5f;
		
		player = GameObject.Find("Player");
        //Choose # of lasers to fire based on difficulty selected
        if(SelectedPlayer.current_difficulty == "easy") lasersToFire = 10;
        else if(SelectedPlayer.current_difficulty == "medium") lasersToFire = 15;
        else if(SelectedPlayer.current_difficulty == "hard") lasersToFire = 20;

		waitForNextLaser = .5f;
		laserTimes = new Laser_Times(laserDuration, waitForNextLaser);
		StartCoroutine(FireLasers());
	}

	private IEnumerator FireLasers() 
	{
		bool switchPosition = !(player.transform.position.x < GameState.end_column / 2);
		float start_X_Position = switchPosition ? 0 : GameState.end_column;
		float end_X_Position = switchPosition ? GameState.end_column : 0;
		
		for(int i = 0; i < lasersToFire; i++) 
		{
			Small_Laser small_laser = Instantiate(smallLaser, new Vector3(start_X_Position, 
				GameState.laneNums[Random.Range(0, GameState.laneNums.Count)] + 0.5f), 
					Quaternion.identity).GetComponent<Small_Laser>();
			small_laser.MoveLaser(new Vector3(end_X_Position, small_laser.transform.position.y), laserTimes);
			yield return new WaitForSeconds(waitForNextLaser);
		}
		yield return new WaitForSeconds(laserTimes.duration + Obstacles.obstacleWaitTime);
		Obstacles.obstacleOn = false;
		Laser_MiniGame.obstacleOn = false;
		Destroy(gameObject);
	}
}
