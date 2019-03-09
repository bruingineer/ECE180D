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
				newX = Mathf.Floor(transform.position.x + 1);
			else
				newX = Mathf.Ceil(transform.position.x - 1);

			newY = Mathf.Ceil(transform.position.y - 1);
			StartCoroutine(MovePlayerPosition(new Vector2(newX, newY) , movementTimeY));
			yield return ChangeColor();
		}
		
	}
}
