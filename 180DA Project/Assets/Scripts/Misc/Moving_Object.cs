using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Moving_Object : MonoBehaviour {
	protected IEnumerator MoveToPosition(Vector3 end_position, float timeToMove)
	{
		var initialPos = transform.position;
		float t = 0f;
		while(t < 1)
		{
				t += Time.deltaTime / timeToMove;
				transform.position = Vector3.Lerp(initialPos, end_position, t);
				yield return null;
		}
		yield return null;
	}

	// checks to see if the game is still playing, if not, the object is destroyed
	protected void CheckGamePlaying()
	{
		if (!GameState.gamePlaying)
			Destroy(gameObject);
	}
}
