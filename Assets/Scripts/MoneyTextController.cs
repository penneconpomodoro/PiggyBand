using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MoneyTextController : MonoBehaviour
{
    private const int durationForTempEarnedMoney = 120;
    private GameDirector gameDirector;
    private int counter;
    private long oldEarnedMoney;
    private long tempEarnedMoney;

    // Start is called before the first frame update
    void Start()
    {
        gameDirector = GameObject.Find("GameDirector").GetComponent<GameDirector>();
        counter = 0;
        oldEarnedMoney = 0;
        tempEarnedMoney = 0;
    }

    // Update is called once per frame
    void Update()
    {
        Text moneyText = GetComponent<Text>();
        moneyText.text = string.Format("ÇΩÇ‹Ç¡ÇΩÇ®ã‡ÅF{0:#,0}â~", gameDirector.earnedMoney);
        if (oldEarnedMoney < gameDirector.earnedMoney)
        {
            counter = durationForTempEarnedMoney;
            tempEarnedMoney = gameDirector.earnedMoney - oldEarnedMoney;
        }
        counter--;
        counter = Mathf.Max(0, counter);
        if (counter > 0)
        {
            moneyText.text += string.Format("\n  ëùÇ¶ÇΩÇ®ã‡ÅF{0:+#,0}â~", tempEarnedMoney);
        }
        oldEarnedMoney = gameDirector.earnedMoney;
    }
}
