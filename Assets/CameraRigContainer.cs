using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRigContainer : MonoBehaviour
{
    private static CameraRigContainer _instance;

    public GameObject cameraRig;
    
    private void Awake()
    {
        _instance = this;
    }

    public static CameraRigContainer Get()
    {
        return _instance;
    }
}
