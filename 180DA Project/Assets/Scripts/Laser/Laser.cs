using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour {
	private LineRenderer lineRenderer;
	Vector3 endPosition;
	public const float laserTime = 0.005f;
	static bool playerHit = false;
	
	public static float destroyLaserDelay = 0.75f;


	void Start () {
		lineRenderer = transform.Find("Beam").GetComponent<LineRenderer>();
		lineRenderer.SetPosition(0, transform.position);
		endPosition = new Vector3(0, transform.position.y);
		StartCoroutine(MoveLaser(endPosition, laserTime));
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

		Destroy(gameObject, destroyLaserDelay);
    }
}
