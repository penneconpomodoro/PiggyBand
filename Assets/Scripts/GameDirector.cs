using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameDirector : MonoBehaviour
{
    public const float durationForReady = 5f;
    public const float durationForGame = 30f;
    public const float extendedDuration = 30f;
    public const float minGameAreaX = -3f;
    public const float maxGameAreaX = 3f;
    public const float minGameAreaY = 0f;
    public const float maxGameAreaY = 8f;

    public AudioClip soundGameStart;
    public AudioClip soundGameOver;
    public AudioClip soundEarnMoney;
    private AudioSource audioSource;

    public enum GameStatus
    {
        Ready,
        Active,
        GameOver
    }

    public long earnedMoney { get; private set; }
    public float timerToFinish { get; private set; }
    public int currentChain { get; private set; }
    public int maxChain { get; private set; }
    public float chainTimer { get; private set; }
    private float resetChainTimer = 3f;
    public GameStatus gameStatus { get; private set; }

    // Start is called before the first frame update
    void Start()
    {
        InitGame();
        audioSource = GetComponent<AudioSource>();
    }

    private void InitGame()
    {
        earnedMoney = 0;
        timerToFinish = durationForReady;
        currentChain = 0;
        maxChain = 0;
        chainTimer = 0f;
        gameStatus = GameStatus.Ready;

        Debug.Log("Are you ready?");
    }

    private void StartGame()
    {
        earnedMoney = 0;
        timerToFinish = durationForGame;
        currentChain = 0;
        maxChain = 0;
        chainTimer = 0f;
        gameStatus = GameStatus.Active;
        PlayOneShotGameStart();

        Debug.Log("Game start!");
    }

    private void PlayOneShotGameStart()
    {
        audioSource.pitch = 1f;
        audioSource.PlayOneShot(soundGameStart);
    }

    private void GameIsOver()
    {
        gameStatus = GameStatus.GameOver;
        PlayOneShotGameOver();

        naichilab.RankingLoader.Instance.SendScoreAndShowRanking(earnedMoney, 0);
        //naichilab.RankingLoader.Instance.SendScoreAndShowRanking(maxChain, 1);

        Debug.Log("Game is over.");
    }

    private void PlayOneShotGameOver()
    {
        audioSource.pitch = 1f;
        audioSource.PlayOneShot(soundGameOver);
    }

    // Update is called once per frame
    void Update()
    {
        if (gameStatus == GameStatus.Ready)
        {
            timerToFinish -= Time.deltaTime;
            timerToFinish = Mathf.Max(0f, timerToFinish);
        }
        else if (gameStatus == GameStatus.Active)
        {
            chainTimer -= Time.deltaTime;
            chainTimer = Mathf.Max(0, chainTimer);
            timerToFinish -= Time.deltaTime;
            timerToFinish = Mathf.Max(0f, timerToFinish);
        }
    }

    private void LateUpdate()
    {
        if (gameStatus == GameStatus.Ready)
        {
            if (timerToFinish <= 0f)
            {
                StartGame();
            }
        }
        else if (gameStatus == GameStatus.Active)
        {
            if (chainTimer <= 0f)
            {
                currentChain = 0;
            }
            if (timerToFinish <= 0f)
            {
                GameIsOver();
            }
        }
    }

    private void CheckEarnedMoneyToExtendTimer(long old)
    {
        int[] thd = new int[] { 100, 1000, 10000, 100000, 1000000 };
        foreach (int i in thd)
        {
            if (old < i && earnedMoney >= i)
            {
                ExtendTimer();
            }
        }
    }

    private void ExtendTimer()
    {
        timerToFinish += extendedDuration;
    }

    public void EarnMoney(int money)
    {
        currentChain++;
        maxChain = Mathf.Max(maxChain, currentChain);
        ResetChainTimer();
        long oldEarnedMoney = earnedMoney;
        earnedMoney += (long)money * currentChain;
        CheckEarnedMoneyToExtendTimer(oldEarnedMoney);
        PlayOneShotEarnMoney();
    }

    private void PlayOneShotEarnMoney()
    {
        audioSource.pitch = Mathf.Clamp(-1f, 1f, 0.2f * (float)currentChain - 1f);
        audioSource.PlayOneShot(soundEarnMoney);
        //        audioSource.pitch = 1f;
    }

    private void ResetChainTimer()
    {
        chainTimer = resetChainTimer;
    }
}