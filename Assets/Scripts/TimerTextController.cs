using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimerTextController : MonoBehaviour
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
        Text text = GetComponent<Text>();
        string title = "残り時間";
        if (gameDirector.gameStatus == GameDirector.GameStatus.Ready)
        {
            title = "開始まで";
        }
        text.text = string.Format("{0}：{1:##0.0}秒", title, gameDirector.TimerToFinish);
    }
}