using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeMarkController : MonoBehaviour
{
    public float rotateX;
    public float rotateY;
    public float rotateZ;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(new Vector3(rotateX, rotateY, rotateZ));
    }
}
