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
		lasersToFire = 15;
		waitForNextLaser = .5f;
		StartCoroutine(FireLasers());
	}

	private IEnumerator FireLasers() 
	{
		float x_position = 0;
		if (player.transform.position.x < GameState.end_column / 2)
		{
			x_position = GameState.end_column;
		}
		for(int i = 0; i < lasersToFire; i++) 
		{
			Instantiate(smallLaser, new Vector3(x_position, 
				GameState.laneNums[Random.Range(0, GameState.laneNums.Count)] + 0.5f), 
					Quaternion.identity);
			yield return new WaitForSeconds(waitForNextLaser);
		}
		yield return new WaitForSeconds(Small_Laser.laserTime + Obstacles.obstacleWaitTime);
		Obstacles.obstacleOn = false;
		Laser_MiniGame.obstacleOn = false;
		Destroy(gameObject);
	}
}
