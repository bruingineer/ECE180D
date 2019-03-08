using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Multiplayer : Player {

	// Change header for Openpose here!
	private string MQTTHeader = "";

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
			StartCoroutine(MovePlayerPosition(new Vector2(transform.position.x, transform.position.y - 1) , movementTimeY));
			yield return ChangeColor();
		}
		
	}
}
