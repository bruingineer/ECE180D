using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Challenge : MonoBehaviour {
	protected abstract void Awake();
	public abstract void StartChallenge();
	protected bool messageOut = false;
	protected string publishTopic;
	protected MQTT_Class multiplayerClient;
	protected virtual void HandleChallengeReady() {}
	public virtual void SetUpClient(string header) {}
	protected void HandleMultiplayer()
	{
		if (GameState_Base.gamePlaying)
		{
			if (!messageOut)
			{
				messageOut = true;
				multiplayerClient.SendMessage(publishTopic, "requested");
				Debug.Log("Sending message to server!");
			}
			HandleChallengeReady();
		}
	}
}
