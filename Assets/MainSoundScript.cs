using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainSoundScript : MonoBehaviour
{
    public bool dontDestroyEnabled = true;

    // Start is called before the first frame update
    void Start()
    {
        if(dontDestroyEnabled)
        {
            DontDestroyOnLoad(this);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
