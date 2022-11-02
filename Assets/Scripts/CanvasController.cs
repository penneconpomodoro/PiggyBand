using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasController : MonoBehaviour
{
    private GameDirector gameDirector;
    public GameObject gameOverUIPrefab;
    private bool isGameOverUIDisplayed;

    // Start is called before the first frame update
    void Start()
    {
        gameDirector = GameObject.Find("GameDirector").GetComponent<GameDirector>();
        isGameOverUIDisplayed = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isGameOverUIDisplayed)
        {
            if (gameDirector.gameStatus == GameDirector.GameStatus.GameOver)
            {
                isGameOverUIDisplayed = true;
                Instantiate(gameOverUIPrefab, this.transform);
            }
        }
    }
}