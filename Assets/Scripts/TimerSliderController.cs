using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimerSliderController : MonoBehaviour
{
    private GameDirector gameDirector;

    public Color minColor;
    public Color maxColor;

    // Start is called before the first frame update
    void Start()
    {
        gameDirector = GameObject.Find("GameDirector").GetComponent<GameDirector>();
    }

    // Update is called once per frame
    void Update()
    {
        Slider slider = GetComponent<Slider>();
        slider.value = gameDirector.TimerToFinish;
        if(gameDirector.gameStatus == GameDirector.GameStatus.Ready)
        {
            slider.maxValue = GameDirector.durationForReady;
        }
        else if (gameDirector.gameStatus == GameDirector.GameStatus.Active)
        {
            slider.maxValue = GameDirector.durationForGame;
            Image image = transform.Find("Fill Area/Fill").GetComponent<Image>();
            image.color = Color.Lerp(minColor, maxColor, (Mathf.Min(slider.maxValue, slider.value) - slider.minValue) / (slider.maxValue - slider.minValue));
        }
    }
}