﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
	Event is a base class for the events in the game (Speech, Gestures)
	that holds info such as the the timer to start the event, initializer,
	and start event function.
*/
public abstract class Event : MonoBehaviour {
	protected Text timeLeft;
	protected Player m_player;
	protected float timerDuration;
	protected float repeatRate;
	protected bool timerStopped;
	protected bool eventCorrect;
	public static float curTime;

	void Awake()
    {
		/*
			Difficulty set depending on the chosen difficulty level
		*/
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
		
		// used to stop the timer immediately after correct or when time runs out
		timerStopped = false;
		// text object to show the timer
		timeLeft = GameObject.FindWithTag("timer").GetComponent<Text>();
		// used as the initializer for each event (gesture, speech, etc)
        Event_Initializer();
		// check to see if the player is in the game, and if so assign the player object to it
		GameObject playerPresent = GameObject.FindWithTag("Player");
		if(playerPresent)
			m_player = GameObject.FindWithTag("Player").GetComponent<Player>();
    }
	/*
		Timer used to start the event. When the timer runs out of time, each events
		HandleIncorrectEvent gets called, and then exits the loop, thus ending the event.
		If the timer is stopped because the event was successful, then after eventCorrect is called, it will
		exit the loop.
	 */
	protected IEnumerator StartTimer()
	{
		// used to reset the time each the event is called
		curTime = timerDuration;
		while (!eventCorrect)
        {   
			if(!timerStopped)
			{
				// deltaTime is used to decrease the time in a natural way
				curTime -= Time.deltaTime;
				// casts the integer time to show on the screen
				int integer = (int)curTime;
				if (integer >= 1)
					timeLeft.text = integer.ToString();
				else
				{
					timeLeft.text = "Time's Up";
					HandleIncorrectEvent();
					break;
				}
			}
            yield return null;
        }
		yield return null;
	}
	
	/*
		Starts the event, first resets the variables, resets states for the actual event, then
		starts the timer to start the event.
	*/
	public IEnumerator StartEvent() 
	{
		timerStopped = false;
		eventCorrect = false;
		SetUpEvent();
		if (SelectedPlayer.current_difficulty != "easy")
			StartCoroutine(MakeTextBlink());
		yield return StartCoroutine(StartTimer());
	}

	// The correct action here moves the player (for the main game)
	protected virtual void HandleCorrectAction() 
	{
		m_player.MovePlayer();
	}

	protected abstract void HandleIncorrectEvent();
	protected abstract IEnumerator MakeTextBlink();
	protected abstract void SetUpEvent();
	protected abstract void HandleCorrectEvent();
	protected abstract void Event_Initializer();
	protected abstract void Reset();
}
