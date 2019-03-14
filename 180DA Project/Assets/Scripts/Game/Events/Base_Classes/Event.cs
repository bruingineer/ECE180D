using System.Collections;
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
    protected Text score;
	protected Player m_player;
	protected float timerDuration;
	protected float repeatRate;
	public static bool timerStopped;
	protected bool eventCorrect;
	public static float curTime;
	private bool handledCleanup = false;
	protected float endDisplayTime = 1.1f;
	private bool timerSoundHandled;
	private AudioClip timerSound;
	private AudioClip eventSetup;
	private AudioClip timerDone;
	private string soundPath = "Sounds/Game Music/";

	protected virtual void Awake()
    {
		timerSoundHandled = false;
		timerSound = Resources.Load<AudioClip>(soundPath + "Timer_Warning");
		eventSetup = Resources.Load<AudioClip>(soundPath + "Event_Setup");
		timerDone = Resources.Load<AudioClip>(soundPath + "timer_done");

		/*
			Difficulty set depending on the chosen difficulty level
		*/
		if (GameState_Base.gameDifficulty == "easy") {
			timerDuration = 11f;
		}
        else if (GameState_Base.gameDifficulty == "medium") {
			timerDuration = 8f;
			repeatRate = .5f;
			
		}
        else if (GameState_Base.gameDifficulty == "hard")  {
			timerDuration = 5f;
			repeatRate = .25f; 
		} 
		
		// used to stop the timer immediately after correct or when time runs out
		timerStopped = false;
		// text object to show the timer
		timeLeft = GameObject.FindWithTag("timer").GetComponent<Text>();

        if (GameState_Base.gameMode == "main_game")
        {
            score = GameObject.FindWithTag("score").GetComponent<Text>();
            SelectedPlayer.new_score = true;
        }

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
		// Debug.Log("fail is: " + SelectedPlayer.current_gesture_fail);
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
				Debug.Log(timerDuration / 2);
				if ((Mathf.Floor(timerDuration / 2) == integer) && !timerSoundHandled)
				{
					timerSoundHandled = true;
					GameState_Base.PlayClip(timerSound);
				}
				if (integer >= 1)
					timeLeft.text = integer.ToString();
				else
				{
					timeLeft.text = "Time's Up";
					GameState_Base.PlayClip(timerDone);
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
	public IEnumerator StartEvent(string phrase = null) 
	{
		timerStopped = false;
		eventCorrect = false;
		timerSoundHandled = false;
		GameState_Base.PlayClip(eventSetup);
		SetUpEvent(phrase);
		// if (SelectedPlayer.current_difficulty != "easy")
			//StartCoroutine(MakeTextBlink());
		yield return StartCoroutine(StartTimer());
	}

	// The correct action here moves the player (for the main game)
	protected virtual void HandleCorrectAction() 
	{
		if (curTime > Mathf.Floor(timerDuration / 2)){
			Powerup.powerup_count++;
		}
		else{
			Powerup.powerup_count = 0;
		}
		m_player.MovePlayer();
	}

	protected virtual void Update()
	{
		if (!GameState_Base.gamePlaying && !handledCleanup)
		{
			handledCleanup = true;
			StopCoroutine("StartEvent");
		}


        if (SelectedPlayer.new_score && GameState_Base.gameMode == "main_game")
        {
            string tot_score = SelectedPlayer.GetResults(true)[0].ToString("#.##");
            Debug.Log("score: " + tot_score);
            score.text = "Score: " + tot_score;
            SelectedPlayer.new_score = false;
        }
	}

	protected abstract void HandleIncorrectEvent();
	protected abstract IEnumerator MakeTextBlink();
	protected abstract void SetUpEvent(string phrase = null);
	protected abstract void HandleCorrectEvent();
	protected abstract void Reset();
	protected abstract IEnumerator ResetCorrect();
}
