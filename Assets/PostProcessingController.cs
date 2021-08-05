using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PostProcessingController : MonoBehaviour
{
    private static PostProcessingController _instance;

    public static PostProcessingController Get()
    {
        return _instance;
    }
    
    private void Awake()
    {
        _instance = this;
    }

    public enum DofLevel
    {
        Low,
        Medium,
        High
    }
    
    public void SetDofLevel(DofLevel level)
    {
        Debug.Log("SetDofLevel: " + level.ToString());
        
        SetDof(10f);
        return;
        switch (level)
        {
            case DofLevel.Low:
                SetDof(10f);
                break;
            case DofLevel.Medium:
                SetDof(47.9f);
                break;
            case DofLevel.High:
                SetDof(76.6f);
                break;
        }
    }

    public void SetDof(float focalLength)
    {
        var volume = GetComponent<Volume>();
        DepthOfField dof;
        
        if (volume.profile.TryGet(out dof))
        {
            dof.focalLength.value = focalLength;
        }
    }
}
