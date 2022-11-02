using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class AllCoinsController : MonoBehaviour
{
    public AudioClip soundAddCoins;
    private AudioSource audioSource;

    public enum CoinStatus
    {
        Stay,
        Moved,
        Falling,
        Changing
    }

    public enum SwitchCoinsResult
    {
        NoCoins,
        Failed,
        Switched
    }

    public GameObject[] prefabs;
    private GameDirector gameDirector;

    private void DeleteAllCoins()
    {
        GameObject[] gameObjects = GameObject.FindGameObjectsWithTag("Coin");
        foreach (var v in gameObjects)
        {
            Destroy(v);
        }
    }

    private GameObject[] GetRandomCoinList()
    {
        List<GameObject> prefabList = new List<GameObject>();
        for (int i = 0; i < 5; i++)
        {
            prefabList.Add(prefabs[0]);
            prefabList.Add(prefabs[2]);
            prefabList.Add(prefabs[4]);
        }
        for (int i = 0; i < 2; i++)
        {
            prefabList.Add(prefabs[1]);
            prefabList.Add(prefabs[3]);
            prefabList.Add(prefabs[5]);
        }

        List<int> index = new List<int>();
        List<GameObject> ret = new List<GameObject>();
        for (int i = 0; i < prefabList.Count(); i++)
        {
            int r = -1;
            while (r == -1 || index.FindIndex(x => x == r) != -1)
            {
                r = Random.Range(0, prefabList.Count());
            }
            index.Add(r);
            ret.Add(prefabList[r]);
        }

        return ret.ToArray();
    }

    private void SetInitCoins()
    {
        List<GameObject> obj = new List<GameObject>();
        obj.AddRange(GetRandomCoinList());
        obj.AddRange(GetRandomCoinList());
        obj.AddRange(GetRandomCoinList());

        for (int y = 0; y < 9; y++)
        {
            for (int x = 0; x < 7; x++)
            {
                GameObject coin = Instantiate(obj[x + y * 7], this.transform);
                coin.GetComponent<CoinController>().InitCoin(x + GameDirector.minGameAreaX, y + GameDirector.minGameAreaY, CoinStatus.Stay);
            }
        }
    }
    private void AddCoins()
    {
        GameObject[] coins = GameObject.FindGameObjectsWithTag("Coin");
        if (coins.Max(x => x.GetComponent<CoinController>().coinY) <= GameDirector.maxGameAreaY - 3f)
        {
            List<GameObject> obj = new List<GameObject>();
            obj.AddRange(GetRandomCoinList());

            for (int y = 0; y < 3; y++)
            {
                for (int x = 0; x < 7; x++)
                {
                    GameObject coin = Instantiate(obj[x + y * 7], this.transform);
                    coin.GetComponent<CoinController>().InitCoin(x + GameDirector.minGameAreaX, y + GameDirector.maxGameAreaY, CoinStatus.Stay);
                }
            }
            PlayOneShotAddCoins();
            Debug.Log("Added coins!");
        }
    }

    private void PlayOneShotAddCoins()
    {
        audioSource.PlayOneShot(soundAddCoins);
    }

    private void Demo()
    {
        for (int i = 0; i < prefabs.Length; i++)
        {
            for (int j = 0; j < 5; j++)
            {
                GameObject obj = Instantiate(prefabs[i], this.transform);
                CoinController coinController = obj.GetComponent<CoinController>();
                coinController.coinX = i - 3;
                coinController.coinY = j;
                coinController.coinType = (CoinController.CoinType)(i + 1);
            }
        }
    }

    private void Init()
    {
        DeleteAllCoins();
        SetInitCoins();
    }

    // Start is called before the first frame update
    void Start()
    {
        gameDirector = GameObject.Find("GameDirector").GetComponent<GameDirector>();
        audioSource = GetComponent<AudioSource>();
        Init();
    }

    // Update is called once per frame
    void Update()
    {
        if (gameDirector.gameStatus == GameDirector.GameStatus.Active)
        {
            AddCoins();
            DropCoins();
            ChainCoins();
        }
    }

    private void DropCoins()
    {
        GameObject[] coins = GameObject.FindGameObjectsWithTag("Coin");
        var groupedCoinsByX = coins.GroupBy(x => x.GetComponent<CoinController>().coinX);
        foreach (var i in groupedCoinsByX)
        {
            var j = i.OrderBy(x => x.GetComponent<CoinController>().coinY);
            float stableY = -1f;
            foreach (var k in j)
            {
                CoinController coinController = k.GetComponent<CoinController>();
                if (stableY + 1f == coinController.coinY)
                {
                    if (coinController.coinStatus == CoinStatus.Falling)
                    {
                        coinController.coinStatus = CoinStatus.Moved;
                    }
                    stableY = coinController.coinY;
                }
                else if (stableY + 1f < coinController.coinY)
                {
                    if (coinController.coinStatus == CoinStatus.Changing)
                    {
                        stableY = coinController.coinY;
                    }
                    else
                    {
                        coinController.coinStatus = CoinStatus.Falling;
                    }
                }
            }
        }
    }

    private void ChainCoins()
    {
        List<List<GameObject>> groupedCoins = GetChainedCoinsGroupedByCoinType();
        int earnedMoney = 0;

        foreach (var v in groupedCoins)
        {
            if (v.Any(x => x.GetComponent<CoinController>().coinStatus == AllCoinsController.CoinStatus.Moved))
            {
                if (v[0].GetComponent<CoinController>().coinType == CoinController.CoinType.Coin1 && v.Count >= 5)
                {
                    foreach (var i in v)
                    {
                        i.GetComponent<CoinController>().SetTargetCoinType(CoinController.CoinType.None);
                    }
                    GameObject y = GetBottomRightCoin(v);
                    y.GetComponent<CoinController>().SetTargetCoinType(CoinController.CoinType.Coin5);

                    earnedMoney += 1 * v.Count;
                }
                else if (v[0].GetComponent<CoinController>().coinType == CoinController.CoinType.Coin5 && v.Count >= 2)
                {
                    foreach (var i in v)
                    {
                        i.GetComponent<CoinController>().SetTargetCoinType(CoinController.CoinType.None);
                    }
                    GameObject y = GetBottomRightCoin(v);
                    y.GetComponent<CoinController>().SetTargetCoinType(CoinController.CoinType.Coin10);

                    earnedMoney += 5 * v.Count;
                }
                else if (v[0].GetComponent<CoinController>().coinType == CoinController.CoinType.Coin10 && v.Count >= 5)
                {
                    foreach (var i in v)
                    {
                        i.GetComponent<CoinController>().SetTargetCoinType(CoinController.CoinType.None);
                    }
                    GameObject y = GetBottomRightCoin(v);
                    y.GetComponent<CoinController>().SetTargetCoinType(CoinController.CoinType.Coin50);

                    earnedMoney += 10 * v.Count;
                }
                else if (v[0].GetComponent<CoinController>().coinType == CoinController.CoinType.Coin50 && v.Count >= 2)
                {
                    foreach (var i in v)
                    {
                        i.GetComponent<CoinController>().SetTargetCoinType(CoinController.CoinType.None);
                    }
                    GameObject y = GetBottomRightCoin(v);
                    y.GetComponent<CoinController>().SetTargetCoinType(CoinController.CoinType.Coin100);

                    earnedMoney += 50 * v.Count;
                }
                else if (v[0].GetComponent<CoinController>().coinType == CoinController.CoinType.Coin100 && v.Count >= 5)
                {
                    foreach (var i in v)
                    {
                        i.GetComponent<CoinController>().SetTargetCoinType(CoinController.CoinType.None);
                    }
                    GameObject y = GetBottomRightCoin(v);
                    y.GetComponent<CoinController>().SetTargetCoinType(CoinController.CoinType.Coin500);

                    earnedMoney += 100 * v.Count;
                }
                else if (v[0].GetComponent<CoinController>().coinType == CoinController.CoinType.Coin500 && v.Count >= 2)
                {
                    foreach (var i in v)
                    {
                        i.GetComponent<CoinController>().SetTargetCoinType(CoinController.CoinType.None);
                    }
                    GameObject y = GetBottomRightCoin(v);
                    y.GetComponent<CoinController>().SetTargetCoinType(CoinController.CoinType.None);

                    earnedMoney += 500 * v.Count;
                }
            }
        }

        if (earnedMoney > 0)
        {
            gameDirector.EarnMoney(earnedMoney);
        }

        static GameObject GetBottomRightCoin(List<GameObject> v)
        {
            float minY = v.Min(x => x.GetComponent<CoinController>().coinY);
            var x = v.Where(a => a.GetComponent<CoinController>().coinY == minY);
            float maxX = x.Max(a => a.GetComponent<CoinController>().coinX);
            var y = x.First(a => a.GetComponent<CoinController>().coinX == maxX);
            return y;
        }
    }

    private List<List<GameObject>> GetChainedCoinsGroupedByCoinType()
    {
        GameObject[] coins = GameObject.FindGameObjectsWithTag("Coin");
        List<List<GameObject>> groupedCoins = new List<List<GameObject>>();

        foreach (GameObject coin in coins)
        {
            AllCoinsController.CoinStatus s = coin.GetComponent<CoinController>().coinStatus;
            if (s == AllCoinsController.CoinStatus.Changing || s == AllCoinsController.CoinStatus.Falling)
            {
                continue;
            }
            CoinController.CoinType t = coin.GetComponent<CoinController>().coinType;
            float x = coin.GetComponent<CoinController>().coinX;
            float y = coin.GetComponent<CoinController>().coinY;

            List<int> groupIndex = new List<int>();
            for (int i = 0; i < groupedCoins.Count; i++)
            {
                bool isSameGroup = false;
                foreach (var j in groupedCoins[i])
                {
                    CoinController.CoinType jt = j.GetComponent<CoinController>().coinType;
                    if (t != jt) break;
                    float jx = j.GetComponent<CoinController>().coinX;
                    float jy = j.GetComponent<CoinController>().coinY;
                    isSameGroup |= ((x == jx) && (Mathf.Abs(y - jy) == 1f));
                    isSameGroup |= ((y == jy) && (Mathf.Abs(x - jx) == 1f));
                    if (isSameGroup) break;
                }
                if (isSameGroup) groupIndex.Add(i);
            }

            // no relevant group
            if (groupIndex.Count == 0)
            {
                groupedCoins.Add(new List<GameObject> { coin });
            }
            else
            {
                groupedCoins[groupIndex[0]].Add(coin);

                // merge into one group when multiple relevant groups exist
                if (groupIndex.Count > 1)
                {
                    for (int i = 1; i < groupIndex.Count; i++)
                    {
                        groupedCoins[groupIndex[0]].AddRange(groupedCoins[groupIndex[i]]);
                        groupedCoins[groupIndex[i]].Clear();
                    }
                }
            }
        }

        return groupedCoins;
    }
    public SwitchCoinsResult SwitchCoins(float srcX, float srcY, UtilityMethod.MovingDirection dir)
    {
        GameObject[] coins = GameObject.FindGameObjectsWithTag("Coin");
        float dstX = srcX;
        float dstY = srcY;
        switch (dir)
        {
            case UtilityMethod.MovingDirection.Left: dstX--; break;
            case UtilityMethod.MovingDirection.Right: dstX++; break;
            case UtilityMethod.MovingDirection.Up: dstY++; break;
            case UtilityMethod.MovingDirection.Down: dstY--; break;
        }

        var v = coins.Where(x => x.GetComponent<CoinController>().coinX == srcX && x.GetComponent<CoinController>().coinY == srcY);
        var w = coins.Where(x => x.GetComponent<CoinController>().coinX == dstX && x.GetComponent<CoinController>().coinY == dstY);

        if (v.Count() > 1 || w.Count() > 1)
        {
            Debug.Log("Multiple coins in player position!!");
            return SwitchCoinsResult.Failed;
        }
        if (v.Count() == 0 && w.Count() == 0)
        {
            Debug.Log("No coins in player position!!");
            return SwitchCoinsResult.NoCoins;
        }
        if (v.Count() == 1 && w.Count() == 0)
        {
            CoinController coinController = v.First().GetComponent<CoinController>();
            if (coinController.coinStatus == AllCoinsController.CoinStatus.Changing || coinController.coinStatus == AllCoinsController.CoinStatus.Falling)
            {
                Debug.Log("Src coin is not stable!!");
                return SwitchCoinsResult.Failed;
            }
            coinController.SetMovingDirection(dir);
            return SwitchCoinsResult.Switched;
        }
        if (v.Count() == 0 && w.Count() == 1)
        {
            CoinController coinController = w.First().GetComponent<CoinController>();
            if (coinController.coinStatus == AllCoinsController.CoinStatus.Changing || coinController.coinStatus == AllCoinsController.CoinStatus.Falling)
            {
                Debug.Log("Dst coin is not stable!!");
                return SwitchCoinsResult.Failed;
            }
            UtilityMethod.MovingDirection objMovDir = UtilityMethod.MovingDirection.NotMoving;
            switch (dir)
            {
                case UtilityMethod.MovingDirection.Left: objMovDir = UtilityMethod.MovingDirection.Right; break;
                case UtilityMethod.MovingDirection.Right: objMovDir = UtilityMethod.MovingDirection.Left; break;
                case UtilityMethod.MovingDirection.Up: objMovDir = UtilityMethod.MovingDirection.Down; break;
                case UtilityMethod.MovingDirection.Down: objMovDir = UtilityMethod.MovingDirection.Up; break;
            }
            coinController.SetMovingDirection(objMovDir);
            return SwitchCoinsResult.Switched;
        }
        if (v.Count() == 1 && w.Count() == 1)
        {
            CoinController srcCoinController = v.First().GetComponent<CoinController>();
            if (srcCoinController.coinStatus == AllCoinsController.CoinStatus.Changing || srcCoinController.coinStatus == AllCoinsController.CoinStatus.Falling)
            {
                Debug.Log("Src coin is not stable!!");
                return SwitchCoinsResult.Failed;
            }

            CoinController dstCoinController = w.First().GetComponent<CoinController>();
            if (dstCoinController.coinStatus == AllCoinsController.CoinStatus.Changing || dstCoinController.coinStatus == AllCoinsController.CoinStatus.Falling)
            {
                Debug.Log("Dst coin is not stable!!");
                return SwitchCoinsResult.Failed;
            }
            srcCoinController.SetMovingDirection(dir);
            UtilityMethod.MovingDirection objMovDir = UtilityMethod.MovingDirection.NotMoving;
            switch (dir)
            {
                case UtilityMethod.MovingDirection.Left: objMovDir = UtilityMethod.MovingDirection.Right; break;
                case UtilityMethod.MovingDirection.Right: objMovDir = UtilityMethod.MovingDirection.Left; break;
                case UtilityMethod.MovingDirection.Up: objMovDir = UtilityMethod.MovingDirection.Down; break;
                case UtilityMethod.MovingDirection.Down: objMovDir = UtilityMethod.MovingDirection.Up; break;
            }
            dstCoinController.SetMovingDirection(objMovDir);
            return SwitchCoinsResult.Switched;
        }
        return SwitchCoinsResult.Failed;
    }
}