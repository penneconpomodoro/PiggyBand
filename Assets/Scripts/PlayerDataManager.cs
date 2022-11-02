using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDataManager : MonoBehaviour
{
    public static PlayerDataManager instance;

    public string version;
    public int bgmVolume;
    public int seVolume;

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
    }

    public void Save()
    {
        CheckData();
        PlayerPrefs.SetString("version", Application.version);
        PlayerPrefs.SetInt("bgmVolume", bgmVolume);
        PlayerPrefs.SetInt("seVolume", seVolume);
        PlayerPrefs.Save();
    }
}