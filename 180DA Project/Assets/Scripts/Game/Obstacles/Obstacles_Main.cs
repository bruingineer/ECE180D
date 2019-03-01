using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacles_Main : Obstacles {

	protected override IEnumerator HandleObstacles()
	{	
		while(GameState_Base.gamePlaying)
		{	
			yield return obstacles[Random.Range(0, obstacles.Count)].StartObstacle();
			Handle_Minigame();
		}
	}
	protected virtual void Handle_Minigame() {}
}
