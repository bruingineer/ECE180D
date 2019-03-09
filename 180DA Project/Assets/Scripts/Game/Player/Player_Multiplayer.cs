using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Multiplayer : Player {

	// Change header for Openpose here!
	private string MQTTHeader = "A";

	protected override void Awake()
	{
		
		playerMQTT_X_topic = MQTTHeader + '/' + localizationTopic;
		playerMQTT_Y_topic = MQTTHeader + '/' + movementTopic;
		base.Awake();
	}

	protected override IEnumerator HandlePlayerHit()
	{
		if (transform.position.y > 0.5)
		{
			StopAllCoroutines();
			float newX, newY;
			if (facingRight)
				newX = Mathf.Floor(transform.position.x + 1) + 0.5f;
			else
				newX = Mathf.Ceil(transform.position.x - 1) - 0.5f;

			// newY = 
			// StartCoroutine(MovePlayerPosition(new Vector2(transform.position.x, transform.position.y - 1) , movementTimeY));
			yield return ChangeColor();
		}
		
	}
}
