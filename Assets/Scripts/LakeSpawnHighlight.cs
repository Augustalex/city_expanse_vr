using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class LakeSpawnHighlight : MonoBehaviour
{
    public LakeSpawn GetLakeSpawn()
    {
        return GetComponentInParent<LakeSpawn>();
    }
}