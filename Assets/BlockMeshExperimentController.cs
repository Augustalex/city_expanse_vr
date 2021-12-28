using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockMeshExperimentController : MonoBehaviour
{
    public Material[] materials;
    private int materialIndex = -1;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            materialIndex += 1;
            if (materialIndex >= materials.Length) materialIndex = 0;
            var material = materials[materialIndex];
            var blockMeshes = FindObjectsOfType<BlockMeshController>();
            Debug.Log("FOUND: " + blockMeshes.Length);
            foreach (var blockMeshController in blockMeshes)
            {
                blockMeshController.SwitchGridType(material);
            }
        }
    }
}