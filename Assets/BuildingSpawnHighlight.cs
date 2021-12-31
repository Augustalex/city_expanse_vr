using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingSpawnHighlight : MonoBehaviour
{
   
    public BuildingSpawn GetBuildingSpawn()
    {
        return GetComponentInParent<BuildingSpawn>();
    }
}
