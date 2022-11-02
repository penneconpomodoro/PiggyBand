using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MaxChainTextController : MonoBehaviour
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
        text.text = string.Format("ç≈ëÂòAçΩêîÅF{0}", gameDirector.maxChain);
    }
}