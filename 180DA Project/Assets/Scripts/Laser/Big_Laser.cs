using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Big_Laser : Laser {
	private LineRenderer lineRenderer;
	Vector3 endPosition;
	static bool playerHit = false;

	void Awake () {
		lineRenderer = GetComponentInChildren<LineRenderer>().transform.GetComponent<LineRenderer>();
		lineRenderer.SetPosition(0, transform.position);
	}
	
	void Update () {
		RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.left, out hit) && !playerHit)
		{
			playerHit = true;
            SelectedPlayer.current_hits += 1;
			Laser_MiniGame.playerWasHit = true;
			hit.transform.GetComponent<Player>().isHit = true;
		}
		if (!GameState.gamePlaying)
			Destroy(gameObject);
	}

	public void MoveLaser(Vector3 endPosition, float timeToMove, float laserDelay)
   	{
		StartCoroutine(MoveLaserToPosition(endPosition, timeToMove, laserDelay));
    }

	private IEnumerator MoveLaserToPosition(Vector3 end_position, float timeToMove, float laserDelay)
   	{
		var initialPos = transform.position;
		float t = 0f;
		while(t < 1)
		{
				t += Time.deltaTime / timeToMove;
				lineRenderer.SetPosition(1, Vector3.Lerp(initialPos, end_position, t));
				yield return null;
		}
		yield return null;
		Destroy(gameObject, laserDelay);
    }
}
