using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinController : MonoBehaviour
{
    public float x;
    public float y;
    public float tracingFactor;
    private UtilityMethod.MovingDirection movingDirection;
    private int fallingCounter;
    private int changingCounter;
    public CoinType coinType;
    private CoinType targetCoinType;
    public GameObject nextLevelCoinPrefab;
    public GameObject changeMarkPrefab;

    public bool IsFalling { get; private set; }
    public bool IsChanging { get; private set; }
    public bool HasMoved { get; private set; }

    public enum CoinType
    {
        None,
        Coin1,
        Coin5,
        Coin10,
        Coin50,
        Coin100,
        Coin500
    }

    // Start is called before the first frame update
    void Start()
    {
        movingDirection = UtilityMethod.MovingDirection.NotMoving;
        //coinStatus = CoinStatus.Stay;
        targetCoinType = coinType;
    }

    // Update is called once per frame
    void Update()
    {
        fallingCounter++;
        if (!IsFalling) fallingCounter = 0;
        changingCounter++;
        if (!IsChanging) changingCounter = 0;
        if (IsFalling)
        {
            if (fallingCounter % 8 == 0)
            {
                movingDirection = UtilityMethod.MovingDirection.Down;
            }
            else
            {
                movingDirection = UtilityMethod.MovingDirection.NotMoving;
            }
        }
    }

    private void LateUpdate()
    {
        switch (movingDirection)
        {
            case UtilityMethod.MovingDirection.Left: x--; break;
            case UtilityMethod.MovingDirection.Right: x++; break;
            case UtilityMethod.MovingDirection.Up: y++; break;
            case UtilityMethod.MovingDirection.Down: y--; break;
        }
        if (movingDirection != UtilityMethod.MovingDirection.NotMoving)
        {
            HasMoved = true;
        }
        // set to default
        movingDirection = UtilityMethod.MovingDirection.NotMoving;

        // interpolated player position

        float tempX = transform.position.x;
        float tempY = transform.position.y;
        tempX = UtilityMethod.GetIntermediateX(tempX, x, tracingFactor, 1e-3f);
        tempY = UtilityMethod.GetIntermediateX(tempY, y, tracingFactor, 1e-3f);
        transform.position = new Vector3(tempX, tempY, 0);

        if (IsChanging)
        {
            if(targetCoinType != CoinType.None && transform.Find("ChangeMark") == null)
            {
                GameObject obj = Instantiate(changeMarkPrefab, transform);
                obj.name = obj.name.Replace("(Clone)", "");
            }
            Color col = GetComponent<SpriteRenderer>().color;
            float a = ((1f + Mathf.Cos(2f * Mathf.PI * (float)changingCounter / 32f)) / 2f);
            //            float a = Mathf.Max(1f - (float)coinStatusCounter / 180f, 0f);
            GetComponent<SpriteRenderer>().color = new Color(col.r, col.g, col.b, a);
            transform.Rotate(new Vector3(0f, 5f, 0f));
            if (changingCounter > 120)
            {
                if (targetCoinType != CoinType.None)
                {
                    GameObject obj = Instantiate(nextLevelCoinPrefab);
                    obj.transform.parent = this.transform.parent;
                    CoinController coinController = obj.GetComponent<CoinController>();
                    coinController.InitCoin(x, y, targetCoinType, true);
                }
                Destroy(gameObject);
            }
        }
    }

    public void InitCoin(float x, float y, bool b)
    {
        this.x = x;
        this.y = y;
        transform.position = new Vector3(x, y, 0);
        IsChanging = false;
        HasMoved = b;
    }

    public void InitCoin(float x, float y, CoinType type, bool b)
    {
        this.x = x;
        this.y = y;
        transform.position = new Vector3(x, y, 0);
        coinType = type;
        IsChanging = false;
        HasMoved = b;
    }

    public void SetMovingDirection(UtilityMethod.MovingDirection direction)
    {
        movingDirection = direction;
    }

    public void SetTargetCoinType(CoinType type)
    {
        targetCoinType = type;
        if (coinType != targetCoinType)
        {
            IsChanging = true;
        }
    }

    public void HasFallen()
    {
        HasMoved = true;
        IsFalling = false;
    }

    public void SetFalling()
    {
        HasMoved = false;
        IsFalling = true;
    }
}