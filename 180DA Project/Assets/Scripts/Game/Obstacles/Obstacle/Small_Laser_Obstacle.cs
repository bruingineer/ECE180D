using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Small_Laser_Obstacle : Laser_Obstacle {

	// Objects
	private GameObject player;

	// Parameters
	// changed based on difficulty in the future
	private int lasersToFire;
	private float waitForNextLaser;
	
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
		player = GameObject.FindWithTag("Player");
		laserTimes = new Laser_Times(laserDuration, waitForNextLaser);
		laserPrefab = (Resources.Load("Prefabs/Lasers/Small_Laser") as GameObject);
	}

	protected override IEnumerator FireLasers(List<int> laserPositions) 
	{
		yield return CreateLasers(laserPositions);
		// wait until last laser ends plus waiting for the next obstacle time
		yield return new WaitForSeconds(laserTimes.duration + obstacleWaitTime);
		
	}

	private IEnumerator CreateLasers(List<int> laserPositions)
	{
		bool switchPosition = !(player.transform.position.y < GameState_Base.end_row / 2);
		float start_Y_Position = switchPosition ? 0 : GameState_Base.end_row;
		float end_Y_Position = switchPosition ? GameState_Base.end_row : 0;
		int list_count = laserPositions.Count;
		for(int i = 0; i < list_count; i++) 
		{
			GameObject prefab = Instantiate(laserPrefab, new Vector3(laserPositions[i] + 0.5f, 
					start_Y_Position), Quaternion.Euler(0, 0, 90));
			Small_Laser smallLaser = AddComponentToLaser(prefab).GetComponent<Small_Laser>();
			if (smallLaser)
				smallLaser.MoveLaser(new Vector3(smallLaser.transform.position.x, end_Y_Position), laserTimes);
				
			yield return new WaitForSeconds(waitForNextLaser);
		}
		yield return null;
	}

	protected override List<int> RandomizeLanes()
	{
		 var rand = new Random();
    	 var rtnlist = new List<int>();

    	for (int i = 0; i < lasersToFire; i++)
    	{
      		rtnlist.Add(Random.Range(0, GameState_Base.numLanes));
    	}
    	return rtnlist;
	}

	protected virtual GameObject AddComponentToLaser(GameObject prefab)
	{
		prefab.AddComponent<Small_Laser>();
		return prefab;
	}
}

public class Small_Laser_Obstacle_Minigame : Small_Laser_Obstacle {
	protected override GameObject AddComponentToLaser(GameObject prefab)
	{
		prefab.AddComponent<Small_Laser_Minigame>();
		return prefab;
	}
}