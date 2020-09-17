using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClosedFistDetector : MonoBehaviour
{
    public Transform palmTransform;
    private bool _closed;
    
    public event Action OpenFist;
    public event Action CloseFist;

    void Update()
    {
        transform.position = palmTransform.position;
    }

    public bool IsOpen()
    {
        return !_closed;
    }
    
    private void OnTriggerEnter(Collider other)
    {
        var isMiddleFinger = other.CompareTag("MiddleFingerTracker");
        if (isMiddleFinger)
        {
            _closed = true;
            OnCloseFist();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        var isMiddleFinger = other.CompareTag("MiddleFingerTracker");
        if (isMiddleFinger)
        {
            _closed = false;
            OnOpenFist();
        }
    }

    private void OnOpenFist()
    {
        OpenFist?.Invoke();
    }

    private void OnCloseFist()
    {
        CloseFist?.Invoke();
    }
}
