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
        var delta = Time.fixedTime - _lastPlacedHouse;
        if (delta > .1f && Random.value < .1)
        {
            var randomValue = Random.value;
            if (randomValue < .5)
            {
                SpawnOneHouse();
            }
            else if (randomValue < .9 && CanSpawnAnotherBigHouse())
            {
                SpawnBigHouse();
            }
            else
            {
                SpawnMegaHouse();
            }
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

    private void SpawnBigHouse()
    {
        var candidates = _worldPlane.GetVacantBlocks()
            .Where(block =>
            {
                var waterBlocks = _worldPlane.GetWaterBlocks();
                return waterBlocks.Count(waterBlock => block.DistanceToOtherBlock(waterBlock) < 2) == 0;
            })
            .Where(block =>
            {
                var blocksWithHouse = _worldPlane
                    .GetBlocksWithHouses();

                return blocksWithHouse.Count(houseBlock => block.DistanceToOtherBlock(houseBlock) <= 3) >= 15;
            })
            .ToList();

        var candidatesCount = candidates.Count;
        if (candidatesCount > 0)
        {
            var vacantLot = candidates[Random.Range(0, candidatesCount)];
            var house = Instantiate(tinyHouseTemplate);
            house.GetComponent<HouseSpawn>().SetToBig();

            vacantLot.Occupy(house);

            _lastPlacedHouse = Time.fixedTime;
        }
    }

    private void SpawnMegaHouse()
    {
        var candidates = _worldPlane.GetVacantBlocks()
            .Where(block =>
            {
                var waterBlocks = _worldPlane.GetWaterBlocks();
                return waterBlocks.Count(waterBlock => block.DistanceToOtherBlock(waterBlock) < 3) == 0;
            })
            .Where(block =>
            {
                var blocksWithHouse = _worldPlane
                    .GetBlocksWithHouses();

                return !blocksWithHouse.Any(houseBlock =>
                           block.DistanceToOtherBlock(houseBlock) <= 1 && houseBlock.GetOccupantHouse().IsSmall())
                       && blocksWithHouse.Count(houseBlock =>
                           block.DistanceToOtherBlock(houseBlock) <= 1 && houseBlock.GetOccupantHouse().IsBig()) >= 2
                       && !blocksWithHouse.Any(houseBlock =>
                           block.DistanceToOtherBlock(houseBlock) <= 1 && houseBlock.GetOccupantHouse().IsMegaBig());
            })
            .ToList();

        var candidatesCount = candidates.Count;
        if (candidatesCount > 0)
        {
            var vacantLot = candidates[Random.Range(0, candidatesCount)];
            var house = Instantiate(tinyHouseTemplate);
            house.GetComponent<HouseSpawn>().SetToMegaBig();

            vacantLot.Occupy(house);

            _lastPlacedHouse = Time.fixedTime;
        }
    }

    public bool CanSpawnAnotherBigHouse()
    {
        var requiredNatureScore = CountBigHouses() * 10; 
        return _worldPlane.NatureScore() > requiredNatureScore;
    }

    public int CountBigHouses()
    {
        return _worldPlane
            .GetBlocksWithHouses()
            .Count(houseBlock => houseBlock.GetOccupantHouse().IsBig());
    }
}