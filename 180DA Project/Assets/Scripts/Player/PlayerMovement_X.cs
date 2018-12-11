using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement_X : MonoBehaviour {

	public AudioClip x_movement;
	void Update () {
       if(!Player.isPlayerMoving && GameState.gamePlaying) MovePlayerX();
	}

	private void MovePlayerX() {
		if (PlayerMQTT_X.playerMoved) {
			Vector3 end_position = new Vector3(transform.position.x + 1, transform.position.y);
			GameState.PlayClip(x_movement);
			PlayerMQTT_X.playerMoved = false;
			StartCoroutine(Player.MoveToPosition(transform, 
				new Vector3(transform.position.x + 1, transform.position.y), Player.movementTimeX));
		}
    } 
}
