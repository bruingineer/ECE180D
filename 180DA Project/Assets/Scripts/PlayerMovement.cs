using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {
	private Rigidbody2D rb;
	const int platform_y_distance = 4;
	const string jump_trigger = "Jump";
	const double end_tile_position_x = 7.5f;
	const float top_position_y = 4.15f;
	const float bottom_position_y = -3.85f;
	private float movementTimeX = .2f;
	private float movementTimeY = .5f;
	private bool isMoving = false;
	private Globals.lane cur_lane;
	Animator m_Animator;
	void Start () {
		cur_lane = PlayerMQTT.lane_state;
		rb = GetComponent<Rigidbody2D>();
		m_Animator = GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update () {
		if(!isMoving && PlayerMQTT.lane_state != cur_lane && validMove()) MovePlayer();
    }
	private IEnumerator MoveToPosition(Vector3 end_position, float timeToMove, bool didJump=false)
   	{
		isMoving = true;
		var currentPos = transform.position;
		var t = 0f;
		while(t < 1)
		{
				t += Time.deltaTime / timeToMove;
				transform.position = Vector3.Lerp(currentPos, end_position, t);
				yield return null;
		}
		if (didJump)
			m_Animator.ResetTrigger(jump_trigger);
		isMoving = false;
    }
	private bool validMove() {
		return Mathf.Abs((int)PlayerMQTT.lane_state - (int)cur_lane) == 1;
	}
	private void movePlayerY(int distance) {
		m_Animator.SetTrigger(jump_trigger);
		StartCoroutine(MoveToPosition(new Vector3(transform.position.x, 
			transform.position.y + distance), movementTimeY, true));
	}
	private void MovePlayer() {
		Globals.lane changed_lane = PlayerMQTT.lane_state;
		int cur_lane_num = (int)cur_lane;
		int changed_lane_num = (int)changed_lane;
		if (Input.GetKeyDown(KeyCode.RightArrow)) {
			if (transform.position.x != end_tile_position_x){
				StartCoroutine(MoveToPosition(new Vector3(transform.position.x + 1, transform.position.y), movementTimeX));
			}
        } else if (Input.GetKeyDown(KeyCode.LeftArrow)) {
			if (transform.position.x != -end_tile_position_x)
            	StartCoroutine(MoveToPosition(new Vector3(transform.position.x - 1, transform.position.y), movementTimeX));
		} else if (changed_lane_num < cur_lane_num && transform.position.y != top_position_y)
			movePlayerY(platform_y_distance);
		else if (changed_lane_num > cur_lane_num && transform.position.y != bottom_position_y)
			movePlayerY(-platform_y_distance);
		cur_lane = changed_lane;
	}
}
