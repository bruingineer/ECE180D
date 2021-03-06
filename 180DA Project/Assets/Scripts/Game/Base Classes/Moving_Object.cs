﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Moving_Object : MonoBehaviour {

	/* 
		Used with all objects that are moving (lasers)
		and it brings them to the specified end position
	*/
	public IEnumerator MoveToPosition(Vector3 end_position, float timeToMove)
	{
		var initialPos = transform.position;
		float t = 0f;
		while(t < 1)
		{
				t += Time.deltaTime / timeToMove;
				// Moves the object position based on time's ratio
				transform.position = Vector3.Lerp(initialPos, end_position, t);
				yield return null;
		}
		yield return null;
	}
}
