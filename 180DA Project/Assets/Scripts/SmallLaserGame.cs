using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmallLaserGame : MonoBehaviour {

	public GameObject smallLaser;
	private int lasersToFire;
	private float waitForNextLaser;

	
	void Start () {
		lasersToFire = 25;
		waitForNextLaser = 0.1f;
		StartCoroutine(FireLasers());
	}

	private IEnumerator FireLasers() 
	{
		for(int i = 0; i < lasersToFire; i++) 
		{
			Instantiate(smallLaser, new Vector3(GameState.end_column, 
				GameState.laneNums[Random.Range(0, GameState.laneNums.Count)] + 0.5f), 
					Quaternion.identity);
			yield return new WaitForSeconds(waitForNextLaser);
		}
		Obstacles.obstacleOn = false;
		yield return new WaitForSeconds(Obstacles.obstacleWaitTime);
		Destroy(gameObject);
	}
}
