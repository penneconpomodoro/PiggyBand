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
        string title = "�c�莞��";
        if (gameDirector.gameStatus == GameDirector.GameStatus.Ready)
        {
            title = "�J�n�܂�";
        }
        text.text = string.Format("{0}�F{1:##0.0}�b", title, gameDirector.TimerToFinish);
    }
}