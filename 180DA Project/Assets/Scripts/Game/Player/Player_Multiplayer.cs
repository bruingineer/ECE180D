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

		if (transform.position.y > 0)
		{
			
			float newX = transform.position.x, newY;
			if (transform.position.x != Mathf.Floor(newX))
			{
				if (facingRight)
					newX = Mathf.Floor(transform.position.x + 1);
				else
					newX = Mathf.Ceil(transform.position.x - 1);
			}

			newY = Mathf.Ceil(transform.position.y - 1);
			Multiplayer_Controller.multiplayerClient.SendMessage(Multiplayer_Controller.playerMovement, newY.ToString());
			StartCoroutine(MovePlayerPosition(new Vector2(newX, newY) , movementTimeY));
			yield return ChangeColor();
		}
		
	}

	protected override void HandlePlayerY(int newY)
	{
		Multiplayer_Controller.multiplayerClient.SendMessage(Multiplayer_Controller.playerMovement, newY.ToString());
	}

	protected override void TeleportPowerUp()
	{
		base.TeleportPowerUp();
		Multiplayer_Controller.multiplayerClient.SendMessage(Multiplayer_Controller.otherPlayerMovement, transform.position.y.ToString());
	}
}
