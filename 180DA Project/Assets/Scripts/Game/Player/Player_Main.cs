using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Main : Player {
	public bool isDead = false;
	private int playerLives = 3;
	
	
	protected override IEnumerator PlayerHitRoutine() 
	{
		isRecovering = true;
		yield return HandlePlayerHit();
		isRecovering = false;
	}

	public override void PlayerHit()
	{
		if(!isRecovering && GameState_Base.gamePlaying)
			StartCoroutine(PlayerHitRoutine());
	}

	protected override IEnumerator HandlePlayerHit()
	{
		SelectedPlayer.current_lives_left--;
		if (playerLives > 1) {
			playerLives--;
			GameObject.Find("Game_Manager").GetComponent<GameState_with_Player>().RemoveLife(playerLives);
			yield return ChangeColor();
		}
		else 
			isDead = true;
	}
}
