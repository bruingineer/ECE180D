using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Main : Player {
	public bool isDead = false;
	private int playerLives = 3;

	protected override void Awake()
	{
		playerMQTT_X_topic = localizationTopic;
		playerMQTT_Y_topic = movementTopic;
		base.Awake();
	}

	protected override IEnumerator HandlePlayerHit()
	{
		SelectedPlayer.current_lives_left--;
		if (playerLives > 1) {
			playerLives--;
			GameObject.Find("Game_Manager").GetComponent<GameState_with_Lives>().RemoveLife(playerLives);
			yield return ChangeColor();
		}
		else 
			isDead = true;
	}
}
