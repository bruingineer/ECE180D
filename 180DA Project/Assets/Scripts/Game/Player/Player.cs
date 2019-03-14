using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Player : Moving_Object {
	// Player Time Parameters
	protected float movementTimeY = .2f;
	private float secondsToMoveX = 0.05f;
	private float recoveryStepTime = 0.5f;
	// must be odd
	private float numberOfChanges = 5;
	protected bool facingRight = true;
	
	// Player Audio Clips
	private AudioClip X_movement;
	private AudioClip Y_movement;
	protected AudioClip playerHitByLaser;
	protected AudioClip playerRecovered;
	private string playerSoundsPath = "Sounds/Player/";

	// Player States
	private bool isPlayerMoving = false;
	protected bool isRecovering = false;

	// Player Objects
	PlayerMQTT_Y m_playerMQTT_Y;
	PlayerMQTT_X m_playerMQTT_X;
	protected const string localizationTopic = "localization";
	protected const string movementTopic = "movement";
	private int playerLaneNum;
	SpriteRenderer sr;

	// MQTT
	protected string playerMQTT_Y_topic;
	protected string playerMQTT_X_topic;

	protected virtual void Awake () {

		m_playerMQTT_Y = new PlayerMQTT_Y(playerMQTT_Y_topic);
		playerLaneNum = (int)transform.position.y;
		m_playerMQTT_X = new PlayerMQTT_X(playerMQTT_X_topic, playerLaneNum);
		sr = gameObject.GetComponent<SpriteRenderer>();
		X_movement = Resources.Load<AudioClip>(playerSoundsPath + "X_Movement");
		Y_movement = Resources.Load<AudioClip>(playerSoundsPath + "Y_Movement");
		playerHitByLaser = Resources.Load<AudioClip>(playerSoundsPath + "Player_Hit");
		playerRecovered = Resources.Load<AudioClip>(playerSoundsPath + "Recover");
	}

	public bool teleportActve = false;
	public bool superActive = false;
	public bool superActived = false;
	void Update()
	{
		if (superActive && !superActived){
			//Debug.Log("super activaed");
				superActived = true;
				StartCoroutine("SetSuper");
		}

		if (!isPlayerMoving) 
			{
				MovePlayerX();
				if (GameState_Base.gamePlaying)
				MovePlayerY();
				if (teleportActve)
					TeleportPowerUp();
			}
	}

	IEnumerator SetSuper(){
			Color p_color = sr.color;
			sr.color = Color.blue;
			yield return new WaitForSeconds(8);
			
			for (int i = 0; i < 16; i++){
				sr.color = Color.blue;
				yield return new WaitForSeconds(0.125f);
				sr.color = p_color;
				yield return new WaitForSeconds(0.125f);
			}
			superActive = false;
			superActived = false;
			Powerup.superOn = false;
	}

	protected virtual void TeleportPowerUp()
	{
		Debug.Log("teleporting");
		isPlayerMoving = true;
		transform.position = Powerup.tele_to;
		
		Debug.Log(Powerup.tele_to);
		teleportActve = false;					
		isPlayerMoving = false;
	}

	private void MovePlayerX() {
		if ((playerLaneNum != m_playerMQTT_X.cur_lane_num)) {
			Vector3 end_position = new Vector3(m_playerMQTT_X.cur_lane_num, transform.position.y );
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

	// add message to send to other
	public IEnumerator MovePlayerPosition(Vector3 end_position, float timeToMove)
   	{
		isPlayerMoving = true;
		yield return MoveToPosition(end_position, timeToMove);
		isPlayerMoving = false;
    }

	private void MovePlayerY() {
			if (m_playerMQTT_Y.PlayerMoved) {
				int newY = (int)(transform.position.y + 1);
				HandlePlayerY(newY);
			}
    } 

	protected virtual void HandlePlayerY(int newY) 
	{
		Vector3 end_position = new Vector3(transform.position.x, newY);
		GameState_Base.PlayClip(Y_movement);
		m_playerMQTT_Y.PlayerMoved = false;
		StartCoroutine(MovePlayerPosition(end_position, movementTimeY));
	}

	// used to change the color when the player is in recovery mode
	public IEnumerator ChangeColor()
	{
		bool playerNormal = false;
		GameState_Base.PlayClip(playerHitByLaser);
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
			GameState_Base.PlayClip(playerRecovered);
		yield return null;
	}

	public void MovePlayer()
	{
		m_playerMQTT_Y.PlayerMoved = true;
	}

	protected IEnumerator PlayerHitRoutine()
	{
		isRecovering = true;
		yield return HandlePlayerHit();
		isRecovering = false;
	}

	public void PlayerHit()
	{
		if(!isRecovering && GameState_Base.gamePlaying)
			StartCoroutine(PlayerHitRoutine());
	}

	protected abstract IEnumerator HandlePlayerHit();
}


