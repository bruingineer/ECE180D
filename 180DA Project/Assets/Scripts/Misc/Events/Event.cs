using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class Event : MonoBehaviour {
	protected Text timeLeft;
	protected Player m_player;
	protected float timerDuration;
	protected float repeatRate;
	private float delay;
	protected bool timerStopped;
	protected bool eventCorrect;

	void Awake()
    {
		if (SelectedPlayer.current_difficulty == "easy") {
			timerDuration = 11f;
		}
        else if (SelectedPlayer.current_difficulty == "medium") {
			timerDuration = 8f;
			repeatRate = .5f;
			
		}
        else if (SelectedPlayer.current_difficulty == "hard")  {
			timerDuration = 5f;
			repeatRate = .25f; 
		} 
		delay = 2f;
		timerStopped = false;
		timeLeft = GameObject.FindWithTag("timer").GetComponent<Text>();
        Event_Initializer();
		// check to see if the player is in the game
		GameObject playerPresent = GameObject.FindWithTag("Player");
		if(playerPresent)
			m_player = GameObject.FindWithTag("Player").GetComponent<Player>();
    }

	protected virtual IEnumerator Delay()
	{
		yield return new WaitForSeconds(delay);
	}

	protected IEnumerator StartTimer()
	{
		float curTime = timerDuration;
		while (!eventCorrect)
        {   
			if(!timerStopped)
			{
				curTime -= Time.deltaTime;
				int integer = (int)curTime;
				if (integer >= 1)
					timeLeft.text = integer.ToString();
				else
				{
					timeLeft.text = "Time's Up";
					timerStopped = true;
					yield return HandleIncorrectEvent();
					break;
				}
			}
            yield return null;
        }
		yield return null;
	}
	
	public IEnumerator StartEvent() 
	{
		timerStopped = false;
		eventCorrect = false;
		SetUpEvent();
		if (SelectedPlayer.current_difficulty != "easy")
			StartCoroutine(MakeTextBlink());
		yield return StartCoroutine(StartTimer());
	}

	protected abstract IEnumerator HandleIncorrectEvent();
	protected abstract IEnumerator MakeTextBlink();
	protected abstract void SetUpEvent();
	protected abstract IEnumerator HandleCorrectEvent();
	protected abstract void Event_Initializer();
	protected abstract void HandleCorrectAction();
}
