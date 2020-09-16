using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudStyleControl : MonoBehaviour
{
    public Material normalMaterial;
    public Material stormMaterial;

    public void SetNormalMaterial()
    {
        gameObject.GetComponentInChildren<MeshRenderer>().material = normalMaterial;
    }

    public void SetStormMaterial()
    {
        gameObject.GetComponentInChildren<MeshRenderer>().material = stormMaterial;
    }
}
