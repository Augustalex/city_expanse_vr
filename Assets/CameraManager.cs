using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{

    public Camera primaryCamera;
    private static CameraManager _instance;

    void Awake()
    {
        _instance = this;
    }

    public static Camera PrimaryCamera()
    {
        return _instance.primaryCamera;
    }
}
