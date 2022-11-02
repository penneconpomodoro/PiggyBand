using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConfigurationButtonManager : MonoBehaviour
{

    [SerializeField] GameObject panel;
    // Start is called before the first frame update
    void Start()
    {
        panel.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Click()
    {
        panel.SetActive(!panel.activeSelf);
        GetComponent<Image>().color = panel.activeSelf ? Color.yellow : Color.white;
        GameObject.Find("PlayButton").GetComponent<Button>().interactable = !panel.activeSelf;
    }
}