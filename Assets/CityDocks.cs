using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class CityDocks : MonoBehaviour
{
    public GameObject dockSpawnTemplate;
    public GameObject boatTemplate;
    
    private WorldPlane _worldPlane;
    private int _boatCount;
    
    void Start()
    {
        _worldPlane = WorldPlane.Get();
    }

    void Update()
    {
        if (Random.value < .1f)
        {
            var houseCount = _worldPlane
                .GetBlocksWithHouses()
                .Count(blockWithHouse => blockWithHouse.GetOccupantHouse().IsBig());
            if (houseCount < 1) return;

            var docks = _worldPlane.GetBlocksWithDocks().Count();
            if (docks > 0)
            {
                if (Random.value < .01f)
                {
                    if (_boatCount < 1)
                    {
                        var waterBlock = _worldPlane.GetWaterBlocks()
                            .Where(block => block.IsGroundLevel())
                            .OrderBy(_ => Random.value)
                            .First();
                        
                        var boat = Instantiate(boatTemplate);
                        var boatPosition = _worldPlane.ToRealCoordinates(waterBlock.GetGridPosition());
                        boatPosition.y = 1.164f;
                        boat.transform.position = boatPosition;

                        _boatCount += 1;
                    }
                }
                
                var docksToHouseRatio = (float) Mathf.Max(docks, 1) / (float) houseCount;
                if (docksToHouseRatio > .05f) return;
            }

            var candidates = _worldPlane.GetWaterBlocks()
                .Where(waterBlock => Math.Abs(waterBlock.GetGridPosition().y) < .5f &&
                                     _worldPlane.GetMajorityBlockTypeWithinRange(waterBlock.GetGridPosition(), 1f)
                                     == Block.BlockType.Water
                                     && _worldPlane.NoNearby(waterBlock.GetGridPosition(), 2f, BlockHasDocksSpawn)
                )
                .SelectMany(waterBlock =>
                {
                    var nearbyBlocks = _worldPlane.GetNearbyBlocks(waterBlock.GetGridPosition()).ToList();
                    if (!nearbyBlocks.Any(otherBlock => otherBlock.IsWater())) return new List<Tuple<Block, Block>>();

                    return nearbyBlocks
                        .Where(block => block.IsOccupable())
                        .Select(block => new Tuple<Block, Block>(waterBlock, block));
                })
                .ToList();

            var candidatesCount = candidates.Count;
            if (candidatesCount > 0)
            {
                var (waterBlock, vacantLot) = candidates[Random.Range(0, candidatesCount)];
                var dockSpawn = Instantiate(dockSpawnTemplate);
                vacantLot.Occupy(dockSpawn);
                var target = waterBlock.transform.position;
                target.y = dockSpawn.transform.position.y;
                dockSpawn.transform.LookAt(target);
            }
        }
    }

    private static bool BlockHasDocksSpawn(Block block)
    {
        if (!block.HasOccupant()) return false;
        var dockSpawn = block.GetOccupant().GetComponent<DocksSpawn>();

        return dockSpawn != null;
    }
}