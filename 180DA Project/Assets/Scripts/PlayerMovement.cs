using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {
	private Rigidbody2D rb;
	const int platform_y_distance = 4;
	const double end_tile_position_x = 7.5f;
	const float top_position_y = 4.15f;
	const float bottom_position_y = -3.85f;
	private float movementTimeX = .2f;
	private float movementTimeY = .5f;
	private bool isMoving = false;
	Animator m_Animator;
	void Start () {
		rb = GetComponent<Rigidbody2D>();
		m_Animator = GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update () {
		if(!isMoving) MovePlayer();
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
		if(didJump)
			m_Animator.ResetTrigger("Jump");
		isMoving = false;
    }
	private void MovePlayer() {
		if (Input.GetKeyDown(KeyCode.RightArrow)) {
			if (transform.position.x != end_tile_position_x){
				StartCoroutine(MoveToPosition(new Vector3(transform.position.x + 1, transform.position.y), movementTimeX));
			}
        } else if (Input.GetKeyDown(KeyCode.LeftArrow)) {
			if (transform.position.x != -end_tile_position_x)
            	StartCoroutine(MoveToPosition(new Vector3(transform.position.x - 1, transform.position.y), movementTimeX));
		} else if (Input.GetKeyDown(KeyCode.UpArrow)) {
			if (transform.position.y != top_position_y) {
				m_Animator.SetTrigger("Jump");
            	StartCoroutine(MoveToPosition(new Vector3(transform.position.x, 
				transform.position.y + platform_y_distance), movementTimeY, true));
			}
		} else if (Input.GetKeyDown(KeyCode.DownArrow)) {
			if (transform.position.y != bottom_position_y){
				m_Animator.SetTrigger("Jump");
            	StartCoroutine(MoveToPosition(new Vector3(transform.position.x, 
				transform.position.y - platform_y_distance), movementTimeY, true));
			}
		}
	}
}
