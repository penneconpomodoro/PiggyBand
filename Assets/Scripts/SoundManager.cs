using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    public enum BgmType
    {
        Main, // ������ 8bit29
        Off = 999,
    }

    public enum SeType
    {
        GameStart, // ����{�^��������24
        GameOver, // ���W�X�^�[�Ő��Z
        EarnMoney, // ����{�^����������4
        PlayerMove, // �J�[�\���ړ�1
        Normal, // �L�����Z��4
        Switching, // �J�[�\���ړ�3
        CoinsSwitched, // �J�[�\���ړ�10
        AddCoins, // �������W�����W����
    }

    private AudioSource bgmAudioSource;
    private AudioSource[] seAudioSource = new AudioSource[16];

    [Range(0, 100)] public int bgmVolume;
    [Range(0, 100)] public int seVolume;
    public AudioClip[] bgmClips;
    public AudioClip[] seClips;
    private BgmType currnetBgmType = BgmType.Main;

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
        bgmAudioSource = gameObject.AddComponent<AudioSource>();
        bgmAudioSource.loop = true;
        for (int i = 0; i < seAudioSource.Length; i++)
        {
            seAudioSource[i] = gameObject.AddComponent<AudioSource>();
        }
        SetBgmVolume(PlayerDataManager.instance.bgmVolume);
        SetSeVolume(PlayerDataManager.instance.seVolume);
        PlayBgm(BgmType.Main);
    }

    // Update is called once per frame
    void Update()
    {
        SetBgmVolume(bgmVolume);
        SetSeVolume(seVolume);
    }

    public void PlayBgm(BgmType t, in bool loop = true)
    {
        currnetBgmType = t;
        bgmAudioSource.loop = loop;
        bgmAudioSource.clip = bgmClips[(int)t];
        bgmAudioSource.Play();
    }

    public void PlaySE(SeType t, in float pitch = 1f)
    {
        if (seAudioSource.All(x => x.isPlaying)) { return; }

        var v = seAudioSource.First(x => !x.isPlaying);
        v.pitch = pitch;
        v.PlayOneShot(seClips[(int)t]);
    }

    public void SetMute(bool b)
    {
        bgmAudioSource.mute = b;
        foreach (var v in seAudioSource)
        {
            v.mute = b;
        }
    }

    public void SetBgmVolume(int volume)
    {
        bgmVolume = volume;
        bgmAudioSource.volume = (float)bgmVolume / 100f;
    }
    public void SetSeVolume(int volume)
    {
        seVolume = volume;
        foreach (var v in seAudioSource)
        {
            v.volume = (float)seVolume / 100f;
        }
    }
}