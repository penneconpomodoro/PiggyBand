using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayButtonScript : MonoBehaviour
{
    private bool isButtonPushed = false;
    private bool isSceneLoaded = false;
    private int counter = 0;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (isButtonPushed)
        {
            counter++;
            Image image = GameObject.Find("Panel").GetComponent<Image>();
            image.enabled = true;
            image.color = new Color(image.color.r, image.color.g, image.color.b, (float)counter * 3f / 255f);
            if (!isSceneLoaded && counter >= 85)
            {
                isSceneLoaded = true;
                SceneManager.LoadScene("GameScene");
            }
        }
    }

    public void Click()
    {
        this.GetComponent<Button>().interactable = false;
        isButtonPushed = true;
    }
}