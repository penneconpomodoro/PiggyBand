using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConfigurationPanelManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        transform.Find("BgmSlider").GetComponent<Slider>().value = PlayerDataManager.instance.bgmVolume;
        transform.Find("SeSlider").GetComponent<Slider>().value = PlayerDataManager.instance.seVolume;
    }

    // Update is called once per frame
    void Update()
    {
        PlayerDataManager.instance.bgmVolume = (int)transform.Find("BgmSlider").GetComponent<Slider>().value;
        SoundManager.instance.bgmVolume = PlayerDataManager.instance.bgmVolume;
        PlayerDataManager.instance.seVolume = (int)transform.Find("SeSlider").GetComponent<Slider>().value;
        SoundManager.instance.seVolume = PlayerDataManager.instance.seVolume;
    }
}
