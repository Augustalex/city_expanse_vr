using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleSpawnHighlight : MonoBehaviour
{
   
    public PuzzleSpawn GetSpawn()
    {
        return GetComponentInParent<PuzzleSpawn>();
    }
}
