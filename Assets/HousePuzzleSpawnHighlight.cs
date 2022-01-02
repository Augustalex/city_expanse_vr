using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HousePuzzleSpawnHighlight : MonoBehaviour
{
   
    public HousePuzzleSpawn GetSpawn()
    {
        return GetComponentInParent<HousePuzzleSpawn>();
    }
}
