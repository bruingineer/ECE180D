using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SmallLaserGame : MonoBehaviour {

	public GameObject smallLaser;
	private int lasersToFire;
	private float waitForNextLaser;
	public GameObject player;

	void Update()
	{
		if (!GameState.gamePlaying)
			Destroy(gameObject);
	}
	
	void Start () {
		player = GameObject.FindGameObjectWithTag("Player");

        //Choose # of lasers to fire based on difficulty selected
        if(SelectedPlayer.current_difficulty == "easy") lasersToFire = 10;
        else if(SelectedPlayer.current_difficulty == "medium") lasersToFire = 15;
        else if(SelectedPlayer.current_difficulty == "hard") lasersToFire = 20;

        waitForNextLaser = .5f;
		StartCoroutine(FireLasers());
	}

	private IEnumerator FireLasers() 
	{
		float start_position = 0;
		float end_position = GameState.end_column;
		if (player.transform.position.x < GameState.end_column / 2)
		{
			start_position = GameState.end_column;
			end_position = 0;
		}
		for(int i = 0; i < lasersToFire; i++) 
		{
			Small_Laser small_laser = Instantiate(smallLaser, new Vector3(start_position, 
				GameState.laneNums[Random.Range(0, GameState.laneNums.Count)] + 0.5f), 
					Quaternion.identity).GetComponent<Small_Laser>();
			small_laser.end_position = end_position;
			
			yield return new WaitForSeconds(waitForNextLaser);
		}
		yield return new WaitForSeconds(Small_Laser.laserTime + Obstacles.obstacleWaitTime);
		Obstacles.obstacleOn = false;
		Laser_MiniGame.obstacleOn = false;
		Destroy(gameObject);
	}
}
