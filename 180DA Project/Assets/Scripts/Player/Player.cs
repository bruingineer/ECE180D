using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour {
	public static Rigidbody2D rb;
	public static float movementTimeX = .2f;
	public static float movementTimeY = .5f;
	public static float playerLaneNum = PlayerMQTT_Y.cur_lane_num;
	public static float secondsToMoveY = 0.1f;
	public static float secondsToMoveX = 0.2f;
	public static bool isPlayerMoving;
	public static bool isHit;
	private bool isRecovering;
	public AudioClip playerHitByLaser;
	public AudioClip playerRecovered;
	public AudioClip playerLost;
	public static float playerRecoveryTime;
	public List<GameObject> playerLifeIcons;
	private int playerLives;
	public GameObject playerExplosion;
	public static bool gameOver;
	void Start () {
		// change x position to be more dynamic
		isRecovering = false;
		playerRecoveryTime = 2.5f;
		transform.position = new Vector3(0.5f, GameState.middle_lane + 0.5f);
		// bool to see if player is moving
		isPlayerMoving = false;
		isHit = false;
		playerLives = 3;
		gameOver = false;
	}

	void Update()
	{
		if (isHit && !isRecovering && !gameOver) {
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
			SpriteRenderer sr = gameObject.GetComponent<SpriteRenderer>();
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
			gameOver = true;
			GameObject explosion = Instantiate(playerExplosion, transform.position, Quaternion.identity);
			ParticleSystem ps = explosion.GetComponent<ParticleSystem>();
			GameState.PlayClip(playerLost);
			float explosionDuration = playerLost.length;
			var main = ps.main;
			main.duration = explosionDuration;
			ps.Play();
			yield return new WaitForSeconds(explosionDuration);
			SceneManager.LoadScene(2);
		}
	}
}


