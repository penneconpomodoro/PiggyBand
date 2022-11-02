using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UtilityMethod
{
    public enum MovingDirection
    {
        NotMoving,
        Left,
        Right,
        Up,
        Down
    }

    public static float GetIntermediateX(float now, float target, float factor, float thd)
    {
        float ret = target;
        if (Mathf.Abs(now - target) > thd)
        {
            ret = now * (1f - factor) + target * factor;
        }
        return ret;
    }
}

public class PlayerController : MonoBehaviour
{
    private GameDirector gameDirector;
    public float playerX;
    public float playerY;
    private float intermediatePlayerX;
    private float intermediatePlayerY;
    public float tracingFactor;
    public PlayerStatus playerStatus;
    private PlayerStatus oldPlayerStatus;
    private UtilityMethod.MovingDirection movingDirection;
    private int playerStatusCounter;
    private AudioSource audioSource;
    public AudioClip soundPlayerMove;
    public AudioClip soundNormal;
    public AudioClip soundSwitching;
    public AudioClip soundCoinsSwitched;

    public enum PlayerStatus
    {
        Normal,
        Switching
    }

    // Start is called before the first frame update
    void Start()
    {
        gameDirector = GameObject.Find("GameDirector").GetComponent<GameDirector>();
        audioSource = GetComponent<AudioSource>();
        playerX = 0f;
        playerY = 0f;
        intermediatePlayerX = 0f;
        intermediatePlayerY = 0f;
        playerStatus = PlayerStatus.Normal;
        oldPlayerStatus = playerStatus;
        movingDirection = UtilityMethod.MovingDirection.NotMoving;
        playerStatusCounter = 0;
        NormalPlayer();
    }

    // Update is called once per frame
    void Update()
    {
        audioSource.mute = gameDirector.gameStatus != GameDirector.GameStatus.Active;
        SetPlayerStatus();
        SetMovingDirection();
    }

    private void SetMovingDirection()
    {
        movingDirection = 0;
        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            movingDirection = UtilityMethod.MovingDirection.Left;
        }
        else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            movingDirection = UtilityMethod.MovingDirection.Right;
        }
        else if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            movingDirection = UtilityMethod.MovingDirection.Up;
        }
        else if (Input.GetKeyDown(KeyCode.Z) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            movingDirection = UtilityMethod.MovingDirection.Down;
        }
    }

    private void SetPlayerStatus()
    {
        oldPlayerStatus = playerStatus;
        if (gameDirector.gameStatus == GameDirector.GameStatus.Active &&
            (Input.GetKey(KeyCode.RightShift) || Input.GetKey(KeyCode.LeftShift)))
        {
            playerStatus = PlayerStatus.Switching;
        }
        else
        {
            playerStatus = PlayerStatus.Normal;
        }

        if (oldPlayerStatus != playerStatus)
        {
            playerStatusCounter = 0;
            if (playerStatus == PlayerStatus.Normal)
            {
                PlayOneShotNormal();
            }
            else if (playerStatus == PlayerStatus.Switching)
            {
                PlayOneShotSwitching();
            }

        }
        else
        {
            playerStatusCounter++;
        }
    }

    private void PlayOneShotPlayerMove()
    {
        audioSource.PlayOneShot(soundPlayerMove);
    }

    private void PlayOneShotNormal()
    {
        audioSource.PlayOneShot(soundNormal);
    }

    private void PlayOneShotSwitching()
    {
        audioSource.PlayOneShot(soundSwitching);
    }
    private void PlayOneShotCoinsSwitched()
    {
        audioSource.PlayOneShot(soundCoinsSwitched);
    }

    private void LateUpdate()
    {
        float oldX = playerX;
        float oldY = playerY;

        switch (movingDirection)
        {
            case UtilityMethod.MovingDirection.Left: playerX--; break;
            case UtilityMethod.MovingDirection.Right: playerX++; break;
            case UtilityMethod.MovingDirection.Up: playerY++; break;
            case UtilityMethod.MovingDirection.Down: playerY--; break;
        }
        playerX = Mathf.Clamp(playerX, GameDirector.minGameAreaX, GameDirector.maxGameAreaX);
        playerY = Mathf.Clamp(playerY, GameDirector.minGameAreaY, GameDirector.maxGameAreaY);

        // interpolated player position
        intermediatePlayerX = UtilityMethod.GetIntermediateX(intermediatePlayerX, playerX, tracingFactor, 1e-3f);
        intermediatePlayerY = UtilityMethod.GetIntermediateX(intermediatePlayerY, playerY, tracingFactor, 1e-3f);
        transform.position = new Vector3(intermediatePlayerX, intermediatePlayerY, 0);

        AllCoinsController.SwitchCoinsResult switchCoins = AllCoinsController.SwitchCoinsResult.NoCoins;

        if (playerStatus == PlayerStatus.Normal)
        {
            NormalPlayer();
        }
        else if (playerStatus == PlayerStatus.Switching)
        {
            SwitchingPlayer();

            if (movingDirection != UtilityMethod.MovingDirection.NotMoving &&
                (oldX == playerX && Mathf.Abs(oldY - playerY) == 1f || (oldY == playerY && Mathf.Abs(oldX - playerX) == 1f)))
            {
                switchCoins = GameObject.Find("Coins").GetComponent<AllCoinsController>().SwitchCoins(oldX, oldY, movingDirection);
            }
        }
        if(movingDirection != UtilityMethod.MovingDirection.NotMoving)
        {
            if(oldX == playerX && oldY == playerY)
            {

            }
            else if(switchCoins== AllCoinsController.SwitchCoinsResult.NoCoins)
            {
                PlayOneShotPlayerMove();
            }
            else if (switchCoins == AllCoinsController.SwitchCoinsResult.Switched)
            {
                PlayOneShotCoinsSwitched();
            }

        }
    }

    private void SetPlayerColor(Color color)
    {
        foreach (string name in new string[] { "PlayerLeft", "PlayerRight", "PlayerTop", "PlayerBottom" })
        {
            GameObject gameObject = GameObject.Find(name);
            SpriteRenderer spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
            spriteRenderer.color = color;
        }
    }

    private void NormalPlayer()
    {
        int i = playerStatusCounter % 64;
        float scale = 1f + 0.125f * Mathf.Sin((float)i / 64f * Mathf.PI);
        SetPlayerColor(new Color(1f, 1f, 1f, 0.5f));
        transform.localScale = new Vector3(scale, scale);
    }

    private void SwitchingPlayer()
    {
        int i = playerStatusCounter % 32;
        float scale = 1f + 0.25f * Mathf.Sin((float)i / 32f * Mathf.PI);
        SetPlayerColor(new Color(1f, 1f, 0f, 0.5f));
        transform.localScale = new Vector3(scale, scale);
    }
}