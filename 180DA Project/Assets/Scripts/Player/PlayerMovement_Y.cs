using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement_Y : MonoBehaviour {
	
	public AudioClip y_movement;

	void Update () {
       if(!Player.isPlayerMoving) MovePlayerY();
    }
	private void MovePlayerY() {
		if ((Player.playerLaneNum != PlayerMQTT_Y.cur_lane_num) && !Player.isDead) {
			Vector3 end_position = new Vector3(transform.position.x, 0.5f + PlayerMQTT_Y.cur_lane_num);
			float timeToMove = Player.secondsToMoveY * Mathf.Abs(Player.playerLaneNum - PlayerMQTT_Y.cur_lane_num);
			GameState.PlayClip(y_movement);
			Player.playerLaneNum = PlayerMQTT_Y.cur_lane_num;
			StartCoroutine(Player.MoveToPosition(transform, end_position, timeToMove));	
		}
	}
}