using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRigContainer : MonoBehaviour
{
    private static CameraRigContainer _instance;

    public GameObject cameraRig;
    public GameObject leftPinkyFinger;
    public GameObject leftMiddleFinger;
    public GameObject rightThumb;

    public enum FingerType
    {
        LeftPinkyFinger,
        LeftMiddleFinger,
        RightThumb
    }
    
    private void Awake()
    {
        _instance = this;
    }

    public static CameraRigContainer Get()
    {
        return _instance;
    }

    public GameObject GetFingerFromType(FingerType fingerType)
    {
        switch (fingerType)
        {
            case FingerType.LeftMiddleFinger:
                return leftMiddleFinger;
            case FingerType.LeftPinkyFinger:
                return leftPinkyFinger;
            case FingerType.RightThumb:
                return rightThumb;
            default:
                return rightThumb;
        }
    }
}
