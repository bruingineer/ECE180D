using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Obstacle : Moving_Object {
	// make dynamic
	protected float obstacleWaitTime = 3f;
	public virtual void StartObstacle() {}
}
