using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
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
    public float X { get; private set; }
    public float Y { get; private set; }
    public float tracingFactor;
    public PlayerStatus playerStatus;
    private PlayerStatus oldPlayerStatus;
    private int playerStatusCounter;
    private AudioSource audioSource;
    public AudioClip soundPlayerMove;
    public AudioClip soundNormal;
    public AudioClip soundSwitching;
    public AudioClip soundCoinsSwitched;
    private float targetX;
    private float targetY;

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
        targetX = 0f;
        targetY = 0f;
        X = 0f;
        Y = 0f;
        transform.position = new Vector3(X, Y, 0);
        playerStatus = PlayerStatus.Normal;
        oldPlayerStatus = playerStatus;
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
        targetX = X;
        targetY = Y;

        if (Mouse.current.leftButton.isPressed)
        {
            Vector3 mousePosition = Input.mousePosition;
            mousePosition.z = -10f;
            Vector3 target = Camera.main.ScreenToWorldPoint(mousePosition);
            targetX = Mathf.Round(target.x);
            targetY = Mathf.Round(target.y);
        }
        // keydown is not available when left click
        else
        {
            if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
            {
                targetX = X - 1f;
            }
            else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.RightArrow))
            {
                targetX = X + 1f;
            }
            else if (Input.GetKeyDown(KeyCode.Z) || Input.GetKeyDown(KeyCode.DownArrow))
            {
                targetY = Y - 1f;
            }
            else if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
            {
                targetY = Y + 1f;
            }
        }
        targetX = Mathf.Clamp(targetX, GameDirector.minGameAreaX, GameDirector.maxGameAreaX);
        targetY = Mathf.Clamp(targetY, GameDirector.minGameAreaY, GameDirector.maxGameAreaY);
    }

    private void SetPlayerStatus()
    {
        oldPlayerStatus = playerStatus;
        playerStatus = PlayerStatus.Normal;
        if (gameDirector.gameStatus == GameDirector.GameStatus.Active)
        {
            if (Input.GetKey(KeyCode.RightShift) || Input.GetKey(KeyCode.LeftShift))
            {
                playerStatus = PlayerStatus.Switching;
            }
            else if (Input.GetMouseButton(0))
            {
                playerStatus = PlayerStatus.Switching;
            }
            else if (Gamepad.current != null)
            {
                if (Gamepad.current.buttonSouth.isPressed)
                {
                    playerStatus = PlayerStatus.Switching;
                }
            }
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
        float oldX = X;
        float oldY = Y;
        X = targetX;
        Y = targetY;

        // interpolated player position
        transform.position = Vector3.Lerp(transform.position, new Vector3(X, Y, 0f), tracingFactor);
        if ((new Vector3(X, Y, 0f) - transform.position).magnitude < 1e-3)
        {
            transform.position = new Vector3(X, Y, 0f);
        }

        AllCoinsController.SwitchCoinsResult switchCoins = AllCoinsController.SwitchCoinsResult.NoCoins;

        if (playerStatus == PlayerStatus.Normal)
        {
            NormalPlayer();
        }
        else if (playerStatus == PlayerStatus.Switching)
        {
            SwitchingPlayer();

            if ((oldX == X && Mathf.Abs(oldY - Y) == 1f || (oldY == Y && Mathf.Abs(oldX - X) == 1f)))
            {
                switchCoins = GameObject.Find("Coins").GetComponent<AllCoinsController>().SwitchCoins(oldX, oldY, X, Y);
            }
        }
        if (oldX == X && oldY == Y)
        {

        }
        else if (switchCoins == AllCoinsController.SwitchCoinsResult.NoCoins)
        {
            PlayOneShotPlayerMove();
        }
        else if (switchCoins == AllCoinsController.SwitchCoinsResult.Switched)
        {
            PlayOneShotCoinsSwitched();
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