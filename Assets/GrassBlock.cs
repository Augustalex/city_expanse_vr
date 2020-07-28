using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrassBlock : MonoBehaviour
{
    public Material normalMaterial;
    public Material topMaterial;

    public void SetNormalMaterial()
    {
        gameObject.GetComponent<MeshRenderer>().material = normalMaterial;
    }

    public void SetTopMaterial()
    {
        gameObject.GetComponent<MeshRenderer>().material = topMaterial;
    }
}
