using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinController : MonoBehaviour
{
    public float coinX;
    public float coinY;
    private float intermediateCoinX;
    private float intermediateCoinY;
    public float tracingFactor;
    private UtilityMethod.MovingDirection movingDirection;
    public AllCoinsController.CoinStatus coinStatus;
    private AllCoinsController.CoinStatus oldCoinStatus;
    private int coinStatusCounter;
    public CoinType coinType;
    private CoinType targetCoinType;
    public GameObject nextLevelCoinPrefab;

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
        intermediateCoinX = coinX;
        intermediateCoinY = coinY;
        movingDirection = UtilityMethod.MovingDirection.NotMoving;
        //coinStatus = CoinStatus.Stay;
        targetCoinType = coinType;
    }

    // Update is called once per frame
    void Update()
    {
        if (oldCoinStatus != coinStatus)
        {
            coinStatusCounter = 0;
        }
        coinStatusCounter++;
        if (coinStatus == AllCoinsController.CoinStatus.Falling)
        {
            if (coinStatusCounter % 8 == 0)
            {
                movingDirection = UtilityMethod.MovingDirection.Down;
            }
            else
            {
                movingDirection = UtilityMethod.MovingDirection.NotMoving;
            }
        }
        else if (coinStatus == AllCoinsController.CoinStatus.Changing)
        {
            movingDirection = UtilityMethod.MovingDirection.NotMoving;
        }
    }

    private void LateUpdate()
    {
        switch (movingDirection)
        {
            case UtilityMethod.MovingDirection.Left: coinX--; break;
            case UtilityMethod.MovingDirection.Right: coinX++; break;
            case UtilityMethod.MovingDirection.Up: coinY++; break;
            case UtilityMethod.MovingDirection.Down: coinY--; break;
        }
        if (movingDirection != UtilityMethod.MovingDirection.NotMoving)
        {
            coinStatus = AllCoinsController.CoinStatus.Moved;
        }
        // set to default
        movingDirection = UtilityMethod.MovingDirection.NotMoving;

        // interpolated player position
        intermediateCoinX = UtilityMethod.GetIntermediateX(intermediateCoinX, coinX, tracingFactor, 1e-3f);
        intermediateCoinY = UtilityMethod.GetIntermediateX(intermediateCoinY, coinY, tracingFactor, 1e-3f);
        transform.position = new Vector3(intermediateCoinX, intermediateCoinY, 0);

        if (coinStatus == AllCoinsController.CoinStatus.Changing)
        {
            Color col = GetComponent<SpriteRenderer>().color;
            float a = ((1f + Mathf.Cos(2f * Mathf.PI * (float)coinStatusCounter / 32f)) / 2f);
            //            float a = Mathf.Max(1f - (float)coinStatusCounter / 180f, 0f);
            GetComponent<SpriteRenderer>().color = new Color(col.r, col.g, col.b, a);
            transform.rotation = Quaternion.Euler(0f, (float)coinStatusCounter * 5f, 0f);
            if (coinStatusCounter > 120)
            {
                if (targetCoinType != CoinType.None)
                {
                    GameObject obj = Instantiate(nextLevelCoinPrefab);
                    obj.transform.parent = this.transform.parent;
                    CoinController coinController = obj.GetComponent<CoinController>();
                    coinController.InitCoin(coinX, coinY, targetCoinType, AllCoinsController.CoinStatus.Moved);
                }
                Destroy(gameObject);
            }
        }
        oldCoinStatus = coinStatus;
    }

    public void InitCoin(float x, float y, AllCoinsController.CoinStatus status)
    {
        coinX = x;
        coinY = y;
        intermediateCoinX = x;
        intermediateCoinY = y;
        transform.position = new Vector3(x, y, 0);
        coinStatus = status;
        oldCoinStatus = status;
    }

    public void InitCoin(float x, float y, CoinType type, AllCoinsController.CoinStatus status)
    {
        coinX = x;
        coinY = y;
        intermediateCoinX = x;
        intermediateCoinY = y;
        transform.position = new Vector3(x, y, 0);
        coinType = type;
        coinStatus = status;
        oldCoinStatus = status;
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
            coinStatus = AllCoinsController.CoinStatus.Changing;
        }
    }
}