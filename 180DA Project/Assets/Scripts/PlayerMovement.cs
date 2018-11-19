using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {
	private Rigidbody2D rb;
	private float movementTimeX = .2f;
	private float movementTimeY = .5f;
	private bool isMoving = false;
	private float playerLaneNum = PlayerMQTT.cur_lane_num;
	private float secondsToMove = 0.1f;
	Animator m_Animator;
	void Start () {
		rb = GetComponent<Rigidbody2D>();
		m_Animator = GetComponent<Animator>();
		// change x position to be more dynamic
		transform.position = new Vector3(-7.5f, 0.5f);
	}
	
	// Update is called once per frame
	void Update () {
		if(!isMoving) MovePlayer();
    }
	private IEnumerator MoveToPosition(int changedLaneNum)
   	{
		Debug.Log(changedLaneNum);
		isMoving = true;
		Vector3 end_position = new Vector3(transform.position.x, 0.5f + (GameState.middle_lane - changedLaneNum));
		var initialPos = transform.position;
		var t = 0f;
		float timeToMove = secondsToMove * Mathf.Abs(playerLaneNum - changedLaneNum);
		while(t < 1)
		{
				t += Time.deltaTime / timeToMove;
				transform.position = Vector3.Lerp(initialPos, end_position, t);
				yield return null;
		}
		playerLaneNum = changedLaneNum;
		isMoving = false;
    }
	private void MovePlayer() {
		if (playerLaneNum != PlayerMQTT.cur_lane_num) {
			StartCoroutine(MoveToPosition(PlayerMQTT.cur_lane_num));	
		}
	}
}

//  if (changed_lane_num < cur_lane_num && transform.position.y != top_position_y)
		// 	movePlayerY(platform_y_distance);
		// else if (changed_lane_num > cur_lane_num && transform.position.y != bottom_position_y)
		// 	movePlayerY(-platform_y_distance);
		// cur_lane = changed_lane;
// if (Input.GetKeyDown(KeyCode.RightArrow)) {
		// 	if (transform.position.x != end_tile_position_x){
		// 		StartCoroutine(MoveToPosition(new Vector3(transform.position.x + 1, transform.position.y), movementTimeX));
		// 	}
        // } else if (Input.GetKeyDown(KeyCode.LeftArrow)) {
		// 	if (transform.position.x != -end_tile_position_x)
        //     	StartCoroutine(MoveToPosition(new Vector3(transform.position.x - 1, transform.position.y), movementTimeX));
		// }
