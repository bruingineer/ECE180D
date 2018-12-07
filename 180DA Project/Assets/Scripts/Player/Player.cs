using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {
	public static float movementTimeX = .2f;
	public static float playerLaneNum = PlayerMQTT_Y.cur_lane_num;
	public static float secondsToMoveY = 0.1f;
	public static bool isPlayerMoving;
	public static bool isHit;
	private bool isRecovering;
	public AudioClip playerHitByLaser;
	public AudioClip playerRecovered;
	public static float playerRecoveryTime;
	public List<GameObject> playerLifeIcons;
	public int playerLives;
	public static bool isDead;
	SpriteRenderer sr;
	void Start () {
		// change x position to be more dynamic
		isRecovering = false;
		playerRecoveryTime = 2.5f;
		transform.position = new Vector3(0.5f, GameState.middle_lane + 0.5f);
		// bool to see if player is moving
		isPlayerMoving = false;
		isHit = false;
		playerLives = 3;
		isDead = false;
		sr = gameObject.GetComponent<SpriteRenderer>();
	}

	void Update()
	{
		if (isHit && !isRecovering && !isDead) {
			StartCoroutine(PlayerHit());
		}
	}
	public static IEnumerator MoveToPosition(Transform player_transform, Vector3 end_position, float timeToMove)
   	{
		isPlayerMoving = true;
		var initialPos = player_transform.position;
		float t = 0f;
		while(t < 1)
		{
				t += Time.deltaTime / timeToMove;
				player_transform.position = Vector3.Lerp(initialPos, end_position, t);
				yield return null;
		}
		isPlayerMoving = false;
    }

	public IEnumerator PlayerHit() 
	{
		if (playerLives > 1) {
			playerLifeIcons[playerLives - 2].SetActive(false);
			playerLives--;
			isRecovering = true;
			bool playerNormal = false;
			GameState.PlayClip(playerHitByLaser);
			for (int i = 0; i < playerRecoveryTime * 2; i++) {
				if (playerNormal) {
					sr.color = new Color(1f, 1f, 1f, 1f);
				} else {
					sr.color = new Color(1f, 1f, 1f, .4f);
				}
				playerNormal = !playerNormal;
				yield return new WaitForSeconds(.5f);
			}
			sr.color = new Color(255f, 255f, 255f, 222f);
			GameState.PlayClip(playerRecovered);
			isRecovering = false;
			isHit = false;
			yield return null;
		}
		else {
			isDead = true;
		}
	}
}


