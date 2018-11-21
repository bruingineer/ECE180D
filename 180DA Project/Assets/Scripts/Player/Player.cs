using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {
	public static Rigidbody2D rb;
	public static float movementTimeX = .2f;
	public static float movementTimeY = .5f;
	public static float playerLaneNum = PlayerMQTT_Y.cur_lane_num;
	public static float secondsToMoveY = 0.1f;
	public static float secondsToMoveX = 0.2f;
	public static AudioSource m_audio_source;
	public static bool isPlayerMoving;
	// Use this for initialization
	void Start () {
		// change x position to be more dynamic
		transform.position = new Vector3(0.5f, GameState.middle_lane + 0.5f);
		// create audio source
		m_audio_source = GetComponent<AudioSource>();
		// bool to see if player is moving
		isPlayerMoving = false;
	}

	public static IEnumerator MoveToPosition(Transform player_transform, Vector3 end_position, float timeToMove)
   	{
		isPlayerMoving = true;
		var initialPos = player_transform.position;
		var t = 0f;
		while(t < 1)
		{
				t += Time.deltaTime / timeToMove;
				player_transform.position = Vector3.Lerp(initialPos, end_position, t);
				yield return null;
		}
		isPlayerMoving = false;
    }
}
