﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using blockInteractions;
using UnityEngine;
using Random = System.Random;

public class OceanHousePuzzleSpawn : PuzzleSpawn
{
    private const int WaterCount = 3;

    public override BuildingInfo GetBuildingInfo()
    {
        return new BuildingInfo {devotees = 10};
    }

    public override Vector3 GetTarget()
    {
        var closestWater = WorldPlane.Get()
            .GetNearbyBlocks(spawnGridPosition)
            .FirstOrDefault(block => block.IsWater());
        return closestWater == null ? Vector3.zero : closestWater.transform.position;
    }

    public override GameObject CreateBuildingAction()
    {
        return BlockFactory.Get().TinyHouse();
    }

    public override bool CanStillConstruct()
    {
        var spawnLot = GetSpawnBlock();
        return spawnLot.IsGrass() && spawnLot.IsVacant() &&
               WorldPlane.Get()
                   .GetNearbyBlocks(spawnLot.GetGridPosition())
                   .Count(block => block.IsWater() && block.GetHeight() == spawnLot.GetHeight()) >=
               WaterCount;
    }
}