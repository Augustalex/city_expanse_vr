using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlatInterfaceController : MonoBehaviour
{
    private static FlatInterfaceController _instance;

    public static FlatInterfaceController Get()
    {
        return _instance;
    }
    
    private void Awake()
    {
        _instance = this;
    }

    public void Disable()
    {
        gameObject.SetActive(false);
    }

    public void Enable()
    {
        gameObject.SetActive(true);
    }
}
