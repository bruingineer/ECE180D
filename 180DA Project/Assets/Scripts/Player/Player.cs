using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Moving_Object {
	// Player Time Parameters
	private float movementTimeX = .2f;
	private float secondsToMoveY = 0.1f;
	private float playerRecoveryTime = 2.5f;
	
	// Player Audio Clips
	public AudioClip y_movement;
	public AudioClip x_movement;
	public AudioClip playerHitByLaser;
	public AudioClip playerRecovered;

	// Player States

	private bool isPlayerMoving = false;
	private bool isRecovering = false;
	public static bool isDead = false;

	// Player Objects

	public float playerLaneNum = PlayerMQTT_Y.cur_lane_num;
	public List<GameObject> playerLifeIcons;
	public int playerLives;
	SpriteRenderer sr;

	void Start () {
		transform.position = new Vector3(0.5f, GameState.middle_lane + 0.5f);
		playerLives = 3;
		isDead = false;
		sr = gameObject.GetComponent<SpriteRenderer>();
	}

	void Update()
	{
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

	private void MovePlayerX() {
		if (PlayerMQTT_X.playerMoved) {
			Vector3 end_position = new Vector3(transform.position.x + 1, transform.position.y);
			GameState.PlayClip(x_movement);
			PlayerMQTT_X.playerMoved = false;
			StartCoroutine(MovePlayerPosition(new Vector3(transform.position.x + 1, transform.position.y), movementTimeX));
		}
    } 

	private IEnumerator PlayerHitRoutine() 
	{
		isRecovering = true;
		if (playerLives > 1) {
			playerLifeIcons[playerLives - 2].SetActive(false);
			playerLives--;
			isRecovering = true;
			GameState.PlayClip(playerHitByLaser);
			yield return ChangeColor();
			GameState.PlayClip(playerRecovered);
			isRecovering = false;
			yield return null;
		}
		else {
			isDead = true;
		}
		isRecovering = false;
	}

	// function used by lasers when it hits the player
	public void PlayerHit()
	{
		if(!isRecovering)
			StartCoroutine(PlayerHitRoutine());
	}

	// used to change the color when the player is in recovery mode
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
			sr.color = new Color(1f, 1f, 1f, 1f);
		yield return null;
	}
}


