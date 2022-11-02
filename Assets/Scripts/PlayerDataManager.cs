using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDataManager : MonoBehaviour
{
    public static PlayerDataManager instance;

    public string version;
    public int bgmVolume;
    public int seVolume;
    public int maxEarnedMoney;
    public int totalEarnedMoney;
    public int maxChain;
    public int numberOfPlays;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        version = PlayerPrefs.GetString("version", Application.version);
        bgmVolume = PlayerPrefs.GetInt("bgmVolume", 30);
        seVolume = PlayerPrefs.GetInt("seVolume", 80);
        maxEarnedMoney = PlayerPrefs.GetInt("maxEarnedMoney", 0);
        totalEarnedMoney = PlayerPrefs.GetInt("totalEarnedMoney", 0);
        maxChain = PlayerPrefs.GetInt("maxChain", 0);
        numberOfPlays = PlayerPrefs.GetInt("numberOfPlays", 0);
        CheckData();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnDestroy()
    {
        Save();
    }

    private void CheckData()
    {
        bgmVolume = Mathf.Clamp(bgmVolume, 0, 100);
        seVolume = Mathf.Clamp(seVolume, 0, 100);
        maxEarnedMoney = Mathf.Clamp(maxEarnedMoney, 0, 999999999);
        totalEarnedMoney = Mathf.Clamp(totalEarnedMoney, 0, 999999999);
        maxChain = Mathf.Clamp(maxChain, 0, 999999999);
        numberOfPlays = Mathf.Clamp(numberOfPlays, 0, 999999999);
    }

    public void Save()
    {
        CheckData();
        PlayerPrefs.SetString("version", Application.version);
        PlayerPrefs.SetInt("bgmVolume", bgmVolume);
        PlayerPrefs.SetInt("seVolume", seVolume);
        PlayerPrefs.SetInt("maxEarnedMoney", maxEarnedMoney);
        PlayerPrefs.SetInt("totalEarnedMoney", totalEarnedMoney);
        PlayerPrefs.SetInt("maxChain", maxChain);
        PlayerPrefs.SetInt("numberOfPlays", numberOfPlays);
        PlayerPrefs.Save();
    }
}