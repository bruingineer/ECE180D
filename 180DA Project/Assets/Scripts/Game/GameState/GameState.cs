using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
using uPLibrary.Networking.M2Mqtt.Utility;
using uPLibrary.Networking.M2Mqtt.Exceptions;
using TMPro;

public abstract class GameState_Base : MonoBehaviour
{

    // Variables
    public static int end_row;
    public static AudioSource gameMusic;
    protected AudioClip gameLostMusic;
    protected AudioClip gameWonMusic;
    private const string SoundsPath = "Sounds/Game Music/";
    public static string gameMode;

    // Objects
    protected Canvas canvas;
    public Text result;
    public Text countdown;
    protected static Button retry, menu;
    public static bool gamePlaying;
    private static GameState_Base instance;
    protected List<Challenge> challenges;

    //MQTT Client to update db when training completed
    MQTTHelper training_client;
    protected int countdownTime = 4;

    protected virtual void Awake()
    {
        challenges = new List<Challenge>();
        training_client = new MQTTHelper("database");
        Time.timeScale = 1;
        instance = this;
        end_row = 14;
        gameLostMusic = Resources.Load<AudioClip>(SoundsPath + "Game_Lost");
        gameWonMusic = Resources.Load<AudioClip>(SoundsPath + "Game_Won");
        gamePlaying = false;
        canvas = GameObject.Find("Canvas").GetComponent<Canvas>();
        gameMusic = GetComponent<AudioSource>();
        retry = Instantiate((Resources.Load("Prefabs/WorldSpace/Retry") as GameObject), canvas.transform).GetComponent<Button>();
        retry.onClick.AddListener(RetryLevel);
        menu = Instantiate((Resources.Load("Prefabs/WorldSpace/Menu") as GameObject), canvas.transform).GetComponent<Button>();
        menu.onClick.AddListener(LoadGameMenu);
        DisableButtons();
		SetUp_Events_Obstacles();
    }

    private IEnumerator StartGameCoroutine()
    {
        yield return StartCoroutine(GameTimer());
        gameMusic.Play();
        yield return new WaitForSeconds(0.1f);
        gamePlaying = true;
        StartChallenges();
    }

    protected virtual void Update()
    {
        if (gamePlaying)
        {
            HandleWin();
            HandleLose();
        }
    }

    public static void StartGame()
    {
        instance.StartCoroutine(instance.StartGameCoroutine());
    }

    public static void PlayClip(AudioClip clip)
    {
        gameMusic.PlayOneShot(clip);
    }

    public IEnumerator GameTimer()
    {
        countdown.gameObject.SetActive(true);
        Text countdownText = countdown.GetComponent<Text>();
        float duration = countdownTime;
        while (duration >= 0)
        {
            duration -= Time.deltaTime;
            int integer = (int)duration;
            if (integer >= 1)
                countdownText.text = integer.ToString();
            else
                countdownText.text = "Start!";
            yield return null;
        }
        HandleCountdown();
    }

    protected virtual void HandleCountdown()
    {
        Destroy(countdown);
    }

    // add these to a scenemanager static class
    public virtual void RetryLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // change to just one static function and constants for scenes
    public void LoadGameMenu()
    {
        menuScene.ChangeScenefromScript("Game Menu");
    }

    protected void LoadStatsMenu()
    {
        menuScene.ChangeScenefromScript("End Game");
    }

    // create function to set up button
    public static void SetUpButtons()
    {
        retry.gameObject.SetActive(true);
        menu.gameObject.SetActive(true);
    }

    public static void DisableButtons()
    {
        retry.gameObject.SetActive(false);
        menu.gameObject.SetActive(false);
    }

    protected IEnumerator HandlePostGame(float length)
    {
        yield return new WaitForSeconds(length);
        HandlePostGameScene();
    }

    protected virtual void GameWon()
    {
        gamePlaying = false;
        result.text = "You win!";
        PlayClip(gameWonMusic);
        training_client.CheckIfTrainingComplete(gameMode);
        StartCoroutine(HandlePostGame(gameWonMusic.length));
    }

    protected void StartChallenges()
    {
        foreach (Challenge challenge in challenges)
            challenge.StartChallenge();
    }

    protected virtual void SetUp_Events_Obstacles()
	{
		StartGame();
	}
    protected abstract void HandlePostGameScene();
    protected abstract void HandleWin();
    protected virtual void HandleLose() { }

}

public abstract class GameState_with_Player : GameState_Base
{

    public GameObject playerExplosion;
    protected GameObject player;
    public static List<int> laneNums;
    public static int numLanes;
    protected Vector3 playerStartPosition;

    protected override void Awake()
    {
        Debug.Log("nice");
        numLanes = 10;
        playerStartPosition = new Vector3(numLanes / 2 + 0.5f, 0.5f);
        InitializeLaneList();
        SelectedPlayer.resetGameStats();
        player = (Resources.Load("Prefabs/Player/Player") as GameObject);
        player = Instantiate(player, playerStartPosition, Quaternion.identity);
        AddComponentToPlayer();
        base.Awake();
    }

    protected abstract void AddComponentToPlayer();

    private void InitializeLaneList()
    {
        laneNums = new List<int>();
        for (int i = 0; i < numLanes; i++)
            laneNums.Add(i);
    }


    protected override void HandleWin()
    {
        if (player.transform.position.y == (end_row - 0.5f))
            GameWon();
    }

    protected void DestroyPlayer()
    {
        gamePlaying = false;
        HandlePlayerDestroyed();
        PlayClip(gameLostMusic);
        ParticleSystem explosion = Instantiate(playerExplosion, player.transform.position, Quaternion.identity).GetComponent<ParticleSystem>();
        float explosionDuration = gameLostMusic.length;
        var main = explosion.main;
        main.duration = explosionDuration;
        explosion.Play();
        StartCoroutine(HandlePostGame(explosionDuration));
    }

    protected virtual void HandlePlayerDestroyed()
    {
        Destroy(player);
    }
}

public abstract class GameState_with_Lives : GameState_with_Player
{

    public List<GameObject> playerIcons;

    public void RemoveLife(int curLives)
    {
        playerIcons[curLives - 1].SetActive(false);
        ChangePitch(curLives);
    }

    public void ChangePitch(int playerLives)
    {
        gameMusic.pitch = playerLives > 1 ? 1 : 1.25f;
    }

    protected override void HandleLose()
    {

        if (player.GetComponent<Player_Main>().isDead)
        {
            SelectedPlayer.died = true;
            result.text = "Game Over!";
            DestroyPlayer();
        }
    }

    protected override void AddComponentToPlayer()
    {
        player.AddComponent<Player_Main>();
    }
}


public abstract class GameState_Event_Minigame : GameState_Base
{
    private int numCorrect = 5;
    public static int curCorrect;

    protected override void Awake()
    {
        curCorrect = 0;
        base.Awake();
    }

    protected override void HandleWin()
    {
        if (curCorrect == numCorrect)
            GameWon();
    }

    protected override void HandlePostGameScene()
    {
        SetUpButtons();
    }
}