using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(WorldPlane))]
public class City : MonoBehaviour
{
    public GameObject tinyHouseTemplate;
    public GameObject sandBlockTemplate;

    private const int SelfSustainedHouses = 3;

    private WorldPlane _worldPlane;
    private double _lastPlacedHouse;
    private bool _sandSpawned;
    private SandSpreadController _sandSpreadController;
    private float _lastPlacedInnerCityHouse;

    void Start()
    {
        _worldPlane = GetComponent<WorldPlane>();
        _lastPlacedHouse = Time.fixedTime - 10;
        _sandSpreadController = SandSpreadController.Get();
    }

    void Update()
    {
        var delta = Time.fixedTime - _lastPlacedHouse;
        if (delta > 10f && Random.value < .1f)
        {
            if (CanSpawnAnotherHouse())
            {
                SpawnOneHouse();
            }
        }

        var innerCityDelta = Time.fixedTime - _lastPlacedInnerCityHouse;
        if (innerCityDelta > 10f && Random.value < .2f)
        {
            if (Random.value < .1f)
            {
                SpawnInnerCityHouse();
            }
        }

        if (Random.value < .01f && HasNoOtherBigHouses())
        {
            SpawnBigHouse();
        }

        if (_sandSpawned
            ? Random.value < _sandSpreadController.startingThreshold
            : Random.value < _sandSpreadController.continuationThreshold)
        {
            var houseCount = _worldPlane.GetBlocksWithHouses().Count;
            if (houseCount > _sandSpreadController.houseCountThreshold)
            {
                if (_worldPlane.CountBlocksOfType(Block.BlockType.Sand) == 0)
                {
                    SpawnSandBlock();
                    _sandSpawned = true;
                }
            }
        }
    }

    private bool HasNoOtherBigHouses()
    {
        var amountOfBigHouses = _worldPlane.GetBlocksWithHouses()
            .Count(block => block.GetOccupantHouse().IsBig());

        return amountOfBigHouses < 2;
    }

    private bool CanSpawnAnotherHouse()
    {
        var bigHouses = _worldPlane.GetBlocksWithHouses().Where(houseBlock => houseBlock.GetOccupantHouse().IsBig())
            .ToList();
        var farms = FarmMasterController.Get().CountFarms();

        var houses = _worldPlane.GetBlocksWithHouses();
        return houses.Count < (SelfSustainedHouses + (bigHouses.Count * 2) + (farms * 2));
    }

    private void SpawnSandBlock()
    {
        var sandBlockRoot = Instantiate(sandBlockTemplate);
        var block = _worldPlane
            .GetVacantBlocks()
            .Where(vacantBlock =>
            {
                var waterBlocks = _worldPlane.GetWaterBlocks();
                return _worldPlane.BlockCanBeReplacedBySandBlock(vacantBlock)
                       && waterBlocks.Count(waterBlock => vacantBlock.DistanceToOtherBlock(waterBlock) < 1) == 0;
            })
            .OrderBy(_ => Random.value)
            .First();
        var sandBlock = sandBlockRoot.GetComponentInChildren<Block>();
        _worldPlane.ReplaceBlock(block, sandBlock);
    }

    private void SpawnOneHouse()
    {
        var candidateCount = 0;

        var candidates = _worldPlane.GetStableWaterBlocks()
            .SelectMany(waterBlock =>
                _worldPlane.GetNearbyVacantLotsStream(waterBlock.GetGridPosition())
                    .Where(waterBlock.IsLevelWith)
                    .Select(block =>
                    {
                        candidateCount += 1;
                        return new Tuple<Block, Block>(waterBlock, block);
                    }));

        if (candidateCount > 0)
        {
            var (waterBlock, vacantLot) = candidates.ElementAt(Random.Range(0, candidateCount));
            var house = Instantiate(tinyHouseTemplate);
            vacantLot.Occupy(house);
            var target = waterBlock.transform.position;
            target.y = house.transform.position.y;
            house.transform.LookAt(target);

            _lastPlacedHouse = Time.fixedTime;
        }
    }

    private void SpawnInnerCityHouse()
    {
        var candidateCount = 0;

        var bigHouseCandidates = _worldPlane.GetBlocksWithHousesStream()
            .Where(block => block.GetOccupantHouse().IsBig())
            .SelectMany(bigHouse => _worldPlane.GetNearbyVacantLotsStream(bigHouse.GetGridPosition())
                .Select(otherBlock =>
                {
                    candidateCount += 1;
                    return new Tuple<Block, Block>(bigHouse, otherBlock);
                }));

        if (candidateCount > 0)
        {
            var (bigHouse, vacantLot) = bigHouseCandidates.ElementAt(Random.Range(0, candidateCount));
            var house = Instantiate(tinyHouseTemplate);
            house.GetComponent<HouseSpawn>().SetIsInnerCity();
            vacantLot.Occupy(house);

            var target = bigHouse.transform.position;
            target.y = house.transform.position.y;
            house.transform.LookAt(target);

            _lastPlacedInnerCityHouse = Time.fixedTime;
        }
    }

    private void SpawnBigHouse()
    {
        var candidates = new List<Block>();

        foreach (var block in _worldPlane.GetBlocksWithHousesStream())
        {
            var occupantHouse = block.GetOccupantHouse();
            if (!occupantHouse.IsSmall()) continue;
            if (!occupantHouse.GoodTimeToUpgrade()) continue;

            // var blocksWithHouse = _worldPlane
            // .GetBlocksWithHouses();

            // var hasLargeEnoughClosePopulation = blocksWithHouse.Count(houseBlock => block.DistanceToOtherBlock(houseBlock) <= 1.5f) >= 3;
            // if (!hasLargeEnoughClosePopulation) return false;

            var hasEnoughSurroundingNature = _worldPlane.NatureScore(block.GetGridPosition(), 8f) > 20;

            if (hasEnoughSurroundingNature)
            {
                candidates.Add(block);
            }
        }

        if (candidates.Count > 0)
        {
            var houseToUpgrade = candidates[Random.Range(0, candidates.Count)];
            houseToUpgrade.GetOccupantHouse().Upgrade();
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
}