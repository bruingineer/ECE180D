using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {
	public float movementTimeX = .2f;
	public AudioClip y_movement;
	public AudioClip x_movement;
	public float playerLaneNum = PlayerMQTT_Y.cur_lane_num;
	public float secondsToMoveY = 0.1f;
	public bool isPlayerMoving;
	public bool isHit;
	private bool isRecovering;
	public AudioClip playerHitByLaser;
	public AudioClip playerRecovered;
	public float playerRecoveryTime;
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
		if (isHit && !isRecovering && !isDead && !GameState.gameWon) {
			StartCoroutine(PlayerHit());
		}

		if(!isPlayerMoving && GameState.gamePlaying) MovePlayerX();
		if(!isPlayerMoving) MovePlayerY();
	}

	private void MovePlayerY() {
		if ((playerLaneNum != PlayerMQTT_Y.cur_lane_num)) {
			Vector3 end_position = new Vector3(transform.position.x, 0.5f + PlayerMQTT_Y.cur_lane_num);
			float timeToMove = secondsToMoveY * Mathf.Abs(playerLaneNum - PlayerMQTT_Y.cur_lane_num);
			GameState.PlayClip(y_movement);
			playerLaneNum = PlayerMQTT_Y.cur_lane_num;
			StartCoroutine(MovePlayerPosition(end_position, timeToMove));	
		}
	}
	public IEnumerator MovePlayerPosition(Vector3 end_position, float timeToMove)
   	{
		isPlayerMoving = true;
		yield return MoveToPosition(end_position, timeToMove);
		isPlayerMoving = false;
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
		yield return null;
	}

	private void MovePlayerX() {
		if (PlayerMQTT_X.playerMoved) {
			Vector3 end_position = new Vector3(transform.position.x + 1, transform.position.y);
			GameState.PlayClip(x_movement);
			PlayerMQTT_X.playerMoved = false;
			StartCoroutine(MovePlayerPosition(new Vector3(transform.position.x + 1, transform.position.y), movementTimeX));
		}
    } 

	public IEnumerator PlayerHit() 
	{
		if (playerLives > 1) {
			playerLifeIcons[playerLives - 2].SetActive(false);
			playerLives--;
			isRecovering = true;
			GameState.PlayClip(playerHitByLaser);
			yield return ChangeColor();
			GameState.PlayClip(playerRecovered);
			isRecovering = false;
			isHit = false;
			yield return null;
		}
		else {
			isDead = true;
		}
	}

	public IEnumerator ChangeColor()
	{
		bool playerNormal = false;
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
		yield return null;
	}
}


