using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PostProcessingController : MonoBehaviour
{
    void Start()
    {
        
    }

    void Update()
    {
        var volume = GetComponent<Volume>();
        DepthOfField dof;
        if (volume.profile.TryGet(out dof))
        {
            Debug.Log("UPDATING!");
            dof.focalLength = new ClampedFloatParameter(10f, 10f, 10f, true);
        }
    }
}
