using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Small_Laser : MonoBehaviour {

	public AudioClip smallLaserSound;
	private float laserTime = 4f;
	void Start () {
		GameState.PlayClip(smallLaserSound);
		StartCoroutine(MoveToPosition(new Vector3(0, transform.position.y), laserTime));
	}

	public IEnumerator MoveToPosition(Vector3 end_position, float timeToMove)
   	{
		var initialPos = transform.position;
		float t = 0f;
		while(t < 1)
		{
				t += Time.deltaTime / timeToMove;
				transform.position = Vector3.Lerp(initialPos, end_position, t);
				yield return null;
		}
		Destroy(gameObject);
		yield return null;
    }
}
