using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CopyrightTextManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Text>().text = string.Format("{0} {1} by {2}", Application.productName, Application.version, Application.companyName);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
