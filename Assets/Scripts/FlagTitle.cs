using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlagTitle : MonoBehaviour
{
    GameObject Camera;
    GameObject Camera2D;
    
    void Start()
    {
        Camera = GameObject.FindWithTag("CameraController");
        Camera2D = GameObject.FindGameObjectWithTag("2DCamera");
    }
    // Update is called once per frame
    void Update()
    {
        if (!Camera.GetComponent<CameraRotation>().isOn2DMode())
        {
            transform.rotation = Camera.transform.rotation;
        }
        else
        {
            transform.rotation = Camera2D.transform.rotation;
        }
    }
}
