using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Moving_Object {
	// Player Time Parameters
	private float movementTimenewy = .2f;
	private float secondsToMovenewX = 0.1f;
	private float playerRecoveryTime = 2.5f;
	private float recoveryStepTime = 0.5f;
	// must be odd
	private float numberOfChanges = 5;
	
	// Player Audio Clips
	public AudioClip newx_movement;
	public AudioClip newy_movement;
	public AudioClip playerHitByLaser;
	public AudioClip playerRecovered;

	// Player States
	private bool isPlayerMoving = false;
	private bool isRecovering = false;
	public bool isDead = false;

	// Player Objects
	PlayerMQTT_newy m_playerMQTT_newy;
	PlayerMQTT_newX m_playerMQTT_newx;
	private int playerLaneNum;
	private int playerLives;
	SpriteRenderer sr;

	// MQTT
	private string playerMQTT_newy_topic = "movement";
	private string playerMQTT_newx_topic = "localization";

	void Awake () {
		m_playerMQTT_newy = new PlayerMQTT_newy(playerMQTT_newy_topic);
		playerLaneNum = (int)transform.position.y;
		m_playerMQTT_newx = new PlayerMQTT_newX(playerMQTT_newx_topic, playerLaneNum);
		playerLives = 3;
		sr = gameObject.GetComponent<SpriteRenderer>();
	}

	void Update()
	{
		Debug.Log(transform.position);
		if(!isPlayerMoving) 
			{
				MovePlayernewY();
				MovePlayernewX();
			}
	}

	private void MovePlayernewX() {
		if ((playerLaneNum != m_playerMQTT_newx.cur_lane_num)) {
			Debug.Log(m_playerMQTT_newx.cur_lane_num);
			Vector3 end_position = new Vector3(0.5f + m_playerMQTT_newx.cur_lane_num, transform.position.y );
			float timeToMove = secondsToMovenewX * Mathf.Abs(playerLaneNum - m_playerMQTT_newx.cur_lane_num);
			GameState_Base.PlayClip(newx_movement);
			playerLaneNum = m_playerMQTT_newx.cur_lane_num;
			StartCoroutine(MovePlayerPosition(end_position, timeToMove));	
		}
	}
	public IEnumerator MovePlayerPosition(Vector3 end_position, float timeToMove)
   	{
		isPlayerMoving = true;
		yield return MoveToPosition(end_position, timeToMove);
		isPlayerMoving = false;
    }

	private void MovePlayernewY() {
		if (m_playerMQTT_newy.PlayerMoved) {
			Vector3 end_position = new Vector3(transform.position.x, transform.position.y + 1);
			GameState_Base.PlayClip(newy_movement);
			m_playerMQTT_newy.PlayerMoved = false;
			StartCoroutine(MovePlayerPosition(end_position, movementTimenewy));
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
		m_playerMQTT_newy.PlayerMoved = true;
	}
}


