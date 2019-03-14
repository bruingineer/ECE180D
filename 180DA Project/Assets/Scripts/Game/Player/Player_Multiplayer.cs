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
			int newY = (int)transform.position.y - 1;
			Multiplayer_Controller.multiplayerClient.SendMessage(Multiplayer_Controller.playerMovement, newY.ToString());
			transform.position =  new Vector2(transform.position.x, newY);
			yield return ChangeColor();
	}

	protected override void HandlePlayerY(int newY)
	{
		base.HandlePlayerY(newY);
		Multiplayer_Controller.multiplayerClient.SendMessage(Multiplayer_Controller.playerMovement, newY.ToString());
	}

	protected override void TeleportPowerUp()
	{
		base.TeleportPowerUp();
		Multiplayer_Controller.multiplayerClient.SendMessage(Multiplayer_Controller.playerMovement, transform.position.y.ToString());
	}
}
