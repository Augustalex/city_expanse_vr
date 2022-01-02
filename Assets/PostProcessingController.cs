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
        SetDof(1f, 1f, 1f);
        return;
        
        switch (level)
        {
            case DofLevel.Low:
                SetDof(1f, 1f, 1f);
                break;
            case DofLevel.Medium:
                SetDof(0.36f, 35.8f, 11.81f);
                break;
            case DofLevel.High:
                SetDof(1.06f, 142f, 29.34f);
                break;
        }
    }

    public void SetDof(float focusDistance, float focalLength, float aperture)
    {
        var volume = GetComponent<Volume>();
        DepthOfField dof;
        
        if (volume.profile.TryGet(out dof))
        {
            dof.focusDistance.value = focusDistance;
            dof.focalLength.value = focalLength;
            dof.aperture.value = aperture;
        }
    }
}
