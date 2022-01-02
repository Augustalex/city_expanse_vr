using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using blockInteractions;
using UnityEngine;
using Random = System.Random;

public class MountainHousePuzzleSpawn : PuzzleSpawn
{
    private const int Height = 2;
    
    public override BuildingInfo GetBuildingInfo()
    {
        return new BuildingInfo {devotees = 10};
    }

    public override Vector3 GetTarget()
    {
        return spawnLookingTarget;
    }

    public override GameObject CreateBuildingAction()
    {
        return BlockFactory.Get().TinyHouse();
    }

    public override bool CanStillConstruct()
    {
        var spawnLot = GetSpawnBlock();
        return spawnLot.IsGrass() && spawnLot.IsVacant() && spawnLot.GetHeight() >= Height;
    }
}