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
    private const int BoatCost = 4;

    void Start()
    {
        _worldPlane = WorldPlane.Get();
    }

    void Update()
    {
        if (Random.value < .01f)
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
                    TrySpawnBoat();
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

    private void TrySpawnBoat()
    {
        if (_boatCount == 1 && Random.value > .1f) return;
        if (_boatCount == 2 && Random.value > .05f) return;
        if (_boatCount == 3 && Random.value > .01f) return;
        if (_boatCount == 4 && Random.value > .001f) return;
        if (_boatCount == 5 && Random.value > .0001f) return;
        if (_boatCount == 6 && Random.value > .00001f) return;
        if (_boatCount == 7 && Random.value > .000001f) return;
        if (_boatCount == 8 && Random.value > .0000001f) return;
        if (_boatCount == 9 && Random.value > .00000001f) return;
        
        var cityWood = CityWoodcutters.Get();
        if (!cityWood.RequireWood(BoatCost)) return;
        cityWood.ConsumeWood(BoatCost);

        var waterBlock = _worldPlane.GetWaterBlocks()
            .Where(block =>
            {
                var blockPosition = block.GetGridPosition();
                
                return block.IsGroundLevel()
                       && _worldPlane.GetBlocksWithDocks()
                           .Any(dock => Vector3.Distance(dock.GetGridPosition(), blockPosition) < 3f);

            })
            .OrderBy(_ => Random.value)
            .First();
                        
        var boat = Instantiate(boatTemplate);
        var boatPosition = _worldPlane.ToRealCoordinates(waterBlock.GetGridPosition());
        boatPosition.y = 1.164f;
        boat.transform.position = boatPosition;

        _boatCount += 1;
    }

    private static bool BlockHasDocksSpawn(Block block)
    {
        if (!block.HasOccupant()) return false;
        var dockSpawn = block.GetOccupant().GetComponent<DocksSpawn>();

        return dockSpawn != null;
    }
}