using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(WorldPlane))]
public class City : MonoBehaviour
{
    public GameObject tinyHouseTemplate;
    private WorldPlane _worldPlane;
    private double _lastPlacedHouse;

    void Start()
    {
        _worldPlane = GetComponent<WorldPlane>();
        _lastPlacedHouse = Time.fixedTime;
    }

    void Update()
    {
        var blocksWithHouse = _worldPlane.GetBlocksWithHouses();
        foreach (var block in blocksWithHouse)
        {
            var houseSpawn = block.GetOccupantHouse();
            if (houseSpawn.IsLarge()) continue;

            var closeHouses = 0;
            foreach (var otherBlock in blocksWithHouse)
            {
                if (otherBlock != block
                    && !otherBlock.GetOccupantHouse().IsLarge()
                    && block.DistanceToOtherBlock(otherBlock) <= 3)
                {
                    closeHouses += 1;
                }
            }

            if (closeHouses >= 5)
            {
                block
                    .GetOccupantHouse()
                    .Upgrade();
            }
        }
        
        var delta = Time.fixedTime - _lastPlacedHouse;
        if (delta > 1 && Random.value < .1)
        {
            SpawnOneHouse();
        }
    }

    private void SpawnOneHouse()
    {
        var candidates = _worldPlane.GetWaterBlocks()
            .SelectMany(waterBlock =>
                _worldPlane.GetNearbyVacantLots(waterBlock.GetPosition())
                    .Select(block => new Tuple<Block, Block>(waterBlock, block)))
            .ToList();

        var candidatesCount = candidates.Count;
        if (candidatesCount > 0)
        {
            var (waterBlock, vacantLot) = candidates[Random.Range(0, candidatesCount)];
            var house = Instantiate(tinyHouseTemplate);
            vacantLot.Occupy(house);
            var target = waterBlock.transform.position;
            target.y = house.transform.position.y;
            house.transform.LookAt(target);
            _lastPlacedHouse = Time.fixedTime;
        }
    }
}