using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour {
	public AudioClip playerHitByLaser;
	private LineRenderer lineRenderer;
	Vector3 endPosition;
	public const float laserTime = 0.01f;
	private bool playerHit;
	void Start () {
		playerHit = false;
		lineRenderer = transform.Find("Beam").GetComponent<LineRenderer>();
		lineRenderer.SetPosition(0, transform.position);
		endPosition = new Vector3(-8f, transform.position.y);
		StartCoroutine(MoveLaser(endPosition, laserTime));
	}
	
	void Update () {
        if (Physics.Raycast(transform.position, Vector3.left) && !playerHit)
        {
			playerHit = true;
            Debug.Log("HIT");
			GameState.PlayClip(playerHitByLaser);
			// take away life here
		}
	}

	private IEnumerator MoveLaser(Vector3 end_position, float timeToMove)
   	{
		Vector3 initialPos = transform.position;
		float t = 0f;
		while(t < 1)
		{
				t += Time.deltaTime / timeToMove;
				lineRenderer.SetPosition(1, Vector3.Lerp(initialPos, end_position, t));
				yield return null;
		}
		Destroy(gameObject, 0.4f);
    }
}
