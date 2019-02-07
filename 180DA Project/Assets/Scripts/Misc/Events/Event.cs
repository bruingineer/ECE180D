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
	protected bool timerPaused = false;

	void Start()
    {
		if (SelectedPlayer.current_difficulty == "easy") {
			timerDuration = 11f;
		}
        else if (SelectedPlayer.current_difficulty == "medium") {
			timerDuration = 5f;
			repeatRate = 2f;
			
		}
        else if (SelectedPlayer.current_difficulty == "hard")  {
			timerDuration = 8f;
			repeatRate = 1f; 
		} 
		delay = 0.75f;
		m_player = GameObject.Find("Player").GetComponent<Player>();
		timeLeft = GameObject.FindWithTag("timer").GetComponent<Text>();
        Initialize();
    }
	protected abstract void Initialize();
	private void SetUpCoroutines() 
	{
		if (SelectedPlayer.current_difficulty != "easy")
			StartCoroutine("MakeTextBlink");

		StartCoroutine(StartTimer());
	}

	protected IEnumerator DelayAndEndTimer()
	{
		yield return new WaitForSeconds(delay);
		StopCoroutine(StartTimer());
	}
	protected IEnumerator StartTimer()
	{
		while (timerDuration >= 0)
        {   
			if(!timerPaused)
			{
				timerDuration -= Time.deltaTime;
				int integer = (int)timerDuration;
				if (integer >= 1)
					timeLeft.text = integer.ToString();
				else
				{
					timeLeft.text = "Time's Up";
					yield return StartCoroutine(HandleIncorrect());
				}
			}

            yield return null;
        }
	}
	
	public IEnumerator StartEvent() 
	{
		SetUpEvent();
		yield return StartCoroutine(StartTimer());
	}

	protected abstract IEnumerator HandleIncorrect();
	protected abstract IEnumerator MakeTextBlink();
	protected abstract void SetUpEvent();
}
