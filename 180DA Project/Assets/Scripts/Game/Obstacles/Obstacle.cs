using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Obstacle : MonoBehaviour {
	// make dynamic
	protected float obstacleWaitTime = 3f;
	public abstract IEnumerator StartObstacle();
}
