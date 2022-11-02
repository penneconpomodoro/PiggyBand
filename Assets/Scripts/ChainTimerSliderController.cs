using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChainTimerSliderController : MonoBehaviour
{
    private GameDirector gameDirector;

    // Start is called before the first frame update
    void Start()
    {
        gameDirector = GameObject.Find("GameDirector").GetComponent<GameDirector>();
    }

    // Update is called once per frame
    void Update()
    {
        Slider slider = GetComponent<Slider>();
        slider.value = gameDirector.ChainTimer;
    }
}