using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrophyMesh : MonoBehaviour
{
    public string trophyName = "Unnamed";
    public Material unveiledMaterial;
    public Material veiledMaterial;

    public int materialIndex = 0;

    public bool
        isDocks = false; // Docks are unfinished, and rather than making something elaborate I'm giving it special treatment here

    private bool _unveiled = false;

    public void Unveil()
    {
        _unveiled = true;
    }

    public void Refresh()
    {
        var meshRenderer = gameObject.GetComponent<MeshRenderer>();

        if (isDocks)
        {
            if (_unveiled)
            {
                UpdateMaterialAtIndex(meshRenderer, unveiledMaterial, 0);
                UpdateMaterialAtIndex(meshRenderer, unveiledMaterial, 1);
            }
            else
            {
                UpdateMaterialAtIndex(meshRenderer, veiledMaterial, 0);
                UpdateMaterialAtIndex(meshRenderer, veiledMaterial, 1);
            }
        }
        else
        {
            if (_unveiled)
            {
                UpdateMaterialAtIndex(meshRenderer, unveiledMaterial, materialIndex);
            }
            else
            {
                UpdateMaterialAtIndex(meshRenderer, veiledMaterial, materialIndex);
            }
        }
    }

    private void UpdateMaterialAtIndex(MeshRenderer meshRenderer, Material material, int index)
    {
        var materialsToUpdate = meshRenderer.materials;
        materialsToUpdate[index] = material;
        meshRenderer.materials = materialsToUpdate;
    }
}