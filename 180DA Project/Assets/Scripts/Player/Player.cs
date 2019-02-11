using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Moving_Object {
	// Player Time Parameters
	private float movementTimeY = .2f;
	private float secondsToMoveX = 0.1f;
	private float playerRecoveryTime = 2.5f;
	private float recoveryStepTime = 0.5f;
	// must be odd
	private float numberOfChanges = 5;
	private bool facingRight = true;
	
	// Player Audio Clips
	public AudioClip X_movement;
	public AudioClip Y_movement;
	public AudioClip playerHitByLaser;
	public AudioClip playerRecovered;

	// Player States
	private bool isPlayerMoving = false;
	private bool isRecovering = false;
	public bool isDead = false;

	// Player Objects
	PlayerMQTT_Y m_playerMQTT_Y;
	PlayerMQTT_X m_playerMQTT_X;
	private int playerLaneNum;
	private int playerLives;
	SpriteRenderer sr;

	// MQTT
	private string playerMQTT_Y_topic = "movement";
	private string playerMQTT_X_topic = "localization";

	void Awake () {
		m_playerMQTT_Y = new PlayerMQTT_Y(playerMQTT_Y_topic);
		playerLaneNum = (int)transform.position.y;
		m_playerMQTT_X = new PlayerMQTT_X(playerMQTT_X_topic, playerLaneNum);
		playerLives = 3;
		sr = gameObject.GetComponent<SpriteRenderer>();
	}

	void Update()
	{
		if(!isPlayerMoving) 
			{
				MovePlayerX();
				if (GameState_Base.gamePlaying)
					MovePlayerY();
			}
	}

	private void MovePlayerX() {
		if ((playerLaneNum != m_playerMQTT_X.cur_lane_num)) {
			Vector3 end_position = new Vector3(0.5f + m_playerMQTT_X.cur_lane_num, transform.position.y );
			float timeToMove = secondsToMoveX * Mathf.Abs(playerLaneNum - m_playerMQTT_X.cur_lane_num);
			GameState_Base.PlayClip(X_movement);
			if ((facingRight && m_playerMQTT_X.cur_lane_num < playerLaneNum)
				|| (!facingRight && m_playerMQTT_X.cur_lane_num > playerLaneNum))
				{
					facingRight = !facingRight;
					transform.Rotate(new Vector3(0, 180, 0));
				}
			playerLaneNum = m_playerMQTT_X.cur_lane_num;
			StartCoroutine(MovePlayerPosition(end_position, timeToMove));	
		}
	}
	public IEnumerator MovePlayerPosition(Vector3 end_position, float timeToMove)
   	{
		isPlayerMoving = true;
		yield return MoveToPosition(end_position, timeToMove);
		isPlayerMoving = false;
    }

	private void MovePlayerY() {
			if (m_playerMQTT_Y.PlayerMoved) {
				Vector3 end_position = new Vector3(transform.position.x, transform.position.y + 1);
				GameState_Base.PlayClip(Y_movement);
				m_playerMQTT_Y.PlayerMoved = false;
				StartCoroutine(MovePlayerPosition(end_position, movementTimeY));
			}
    } 

	private IEnumerator PlayerHitRoutine() 
	{
		SelectedPlayer.current_hits++;
		isRecovering = true;
		if (playerLives > 1) {
			playerLives--;
			// create object for game_manager
			GameObject.Find("Game_Manager").GetComponent<GameState_with_Player>().RemoveLife(playerLives);
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
		if(!isRecovering && GameState_Base.gamePlaying)
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
		m_playerMQTT_Y.PlayerMoved = true;
	}
}


