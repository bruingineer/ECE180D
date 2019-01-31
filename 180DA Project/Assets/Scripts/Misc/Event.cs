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
        SetUp();
        SetUpCoroutines();
    }

	protected virtual void SetUp() {}
	private void SetUpCoroutines() 
	{
		StartCoroutine("Timer");
		if (SelectedPlayer.current_difficulty != "easy")
			StartCoroutine("MakeTextBlink");
        
	}

	protected abstract void HandleIncorrect();
	protected void HandleCorrect()
	{
		StopCoroutine("Timer");
		m_player.MovePlayer();
	}
	protected virtual IEnumerator MakeTextBlink() {yield return null;}
	protected IEnumerator DelayAndDestroy()
	{
		yield return new WaitForSeconds(delay);
		Destroy(gameObject);
	}
	protected IEnumerator Timer()
	{
		while (timerDuration >= 0)
        {   
            timerDuration -= Time.deltaTime;
            int integer = (int)timerDuration;
            if (integer >= 1)
                timeLeft.text = integer.ToString();
            else
			{
				timeLeft.text = "Time's Up";
				HandleIncorrect();
            	Destroy(gameObject);
			}
            yield return null;
        }
	}
}
