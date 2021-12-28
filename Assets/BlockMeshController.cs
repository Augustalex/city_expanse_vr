using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockMeshController : MonoBehaviour
{
    public void SwitchGridType(Material newMaterial)
    {
        var renderers = GetComponentsInChildren<MeshRenderer>();
        foreach (var meshRenderer in renderers)
        {
            var materials = new Material[]
            {
                newMaterial
            };
            meshRenderer.materials = materials;
        }
    }
}
