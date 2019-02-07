using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Moving_Object {
	// Player Time Parameters
	private float movementTimeX = .2f;
	private float secondsToMoveY = 0.1f;
	private float playerRecoveryTime = 2.5f;
	private float recoveryStepTime = 0.5f;
	// must be odd
	private float numberOfChanges = 5;
	
	// Player Audio Clips
	public AudioClip y_movement;
	public AudioClip x_movement;
	public AudioClip playerHitByLaser;
	public AudioClip playerRecovered;

	// Player States
	private bool isPlayerMoving = false;
	private bool isRecovering = false;
	public bool isDead = false;

	// Player Objects
	PlayerMQTT_X m_playerMQTT_x;
	PlayerMQTT_Y m_playerMQTT_y;
	private int playerLaneNum;
	private int playerLives;
	SpriteRenderer sr;

	// MQTT
	private string playerMQTT_X_topic = "movement";
	private string playerMQTT_Y_topic = "localization";

	void Awake () {
		m_playerMQTT_x = new PlayerMQTT_X(playerMQTT_X_topic);
		playerLaneNum = (int)transform.position.x;
		m_playerMQTT_y = new PlayerMQTT_Y(playerMQTT_Y_topic, playerLaneNum);
		playerLives = 3;
		sr = gameObject.GetComponent<SpriteRenderer>();
	}

	void Update()
	{
		if(!isPlayerMoving) 
			{
				MovePlayerX();
				MovePlayerY();
			}
	}

	private void MovePlayerY() {
		if ((playerLaneNum != m_playerMQTT_y.cur_lane_num)) {
			Vector3 end_position = new Vector3(transform.position.x, 0.5f + m_playerMQTT_y.cur_lane_num);
			float timeToMove = secondsToMoveY * Mathf.Abs(playerLaneNum - m_playerMQTT_y.cur_lane_num);
			GameState_Base.PlayClip(y_movement);
			playerLaneNum = m_playerMQTT_y.cur_lane_num;
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
		if (m_playerMQTT_x.PlayerMoved) {
			Vector3 end_position = new Vector3(transform.position.x + 1, transform.position.y);
			GameState_Base.PlayClip(x_movement);
			m_playerMQTT_x.PlayerMoved = false;
			StartCoroutine(MovePlayerPosition(new Vector3(transform.position.x + 1, transform.position.y), movementTimeX));
		}
    } 

	private IEnumerator PlayerHitRoutine() 
	{
		SelectedPlayer.current_hits++;
		isRecovering = true;
		if (playerLives > 1) {
			playerLives--;
			isRecovering = true;
			GameState_Base.PlayClip(playerHitByLaser);
			yield return ChangeColor();
			GameState_Base.PlayClip(playerRecovered);
			isRecovering = false;
			yield return null;
		}
		else 
			isDead = true;
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
		for (int i = 0; i < numberOfChanges; i++) {
				if (playerNormal) {
					sr.color = new Color(1f, 1f, 1f, 1f);
				} else {
					sr.color = new Color(1f, 1f, 1f, .4f);
				}
				playerNormal = !playerNormal;
				yield return new WaitForSeconds(recoveryStepTime);
			}
			sr.color = new Color(1f, 1f, 1f, 1f);
		yield return null;
	}

	public void MovePlayer()
	{
		m_playerMQTT_x.PlayerMoved = true;
	}
}


