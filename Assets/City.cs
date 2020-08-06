using System;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(WorldPlane))]
public class City : MonoBehaviour
{
    public GameObject tinyHouseTemplate;
    public GameObject sandBlockTemplate;
    private WorldPlane _worldPlane;
    private double _lastPlacedHouse;
    private double _lastUpgradedBigHouse;
    private bool _sandSpawned;
    private SandSpreadController _sandSpreadController;

    void Start()
    {
        _worldPlane = GetComponent<WorldPlane>();
        _lastPlacedHouse = Time.fixedTime - 10;
        _lastUpgradedBigHouse = Time.fixedTime;
        _sandSpreadController = FindObjectOfType<SandSpreadController>();
    }

    void Update()
    {
        var delta = Time.fixedTime - _lastPlacedHouse;
        if (delta > 10f && Random.value < .1)
        {
            SpawnOneHouse();
        }

        var upgradeDelta = Time.fixedTime - _lastUpgradedBigHouse;
        if (upgradeDelta > 1.5f)
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

    private void SpawnSandBlock()
    {
        var sandBlockRoot = Instantiate(sandBlockTemplate);
        var block = _worldPlane.GetVacantBlocks()
            .OrderBy(_ => Random.value)
            .First();
        var sandBlock = sandBlockRoot.GetComponentInChildren<Block>();
        _worldPlane.ReplaceBlock(block, sandBlock);
    }

    private void SpawnOneHouse()
    {
        var candidates = _worldPlane.GetWaterBlocks()
            .SelectMany(waterBlock =>
                _worldPlane.GetNearbyVacantLots(waterBlock.GetGridPosition())
                    .Select(block => new Tuple<Block, Block>(waterBlock, block)))
            .ToList();
        var bigHouseCandidates = _worldPlane.GetBlocksWithHouses()
            .Where(block => block.GetOccupantHouse().IsBig())
            .SelectMany(block => _worldPlane.GetNearbyVacantLots(block.GetGridPosition())
                .Select(otherBlock => new Tuple<Block, Block>(block, otherBlock)))
            .ToList();

        var selectCandidates = bigHouseCandidates.Count > 0 ? bigHouseCandidates : candidates;
        var candidatesCount = selectCandidates.Count;
        if (candidatesCount > 0)
        {
            var (waterBlock, vacantLot) = selectCandidates[Random.Range(0, candidatesCount)];
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
        var candidates = _worldPlane.GetBlocksWithHouses()
            .Where(block =>
            {
                var occupantHouse = block.GetOccupantHouse();
                if (!occupantHouse.IsSmall()) return false;
                if (!occupantHouse.GoodTimeToUpgrade()) return false;
                // var blocksWithHouse = _worldPlane
                // .GetBlocksWithHouses();

                // var hasLargeEnoughClosePopulation = blocksWithHouse.Count(houseBlock => block.DistanceToOtherBlock(houseBlock) <= 1.5f) >= 3;
                // if (!hasLargeEnoughClosePopulation) return false;

                var hasEnoughSurroundingNature = _worldPlane.NatureScore(block.GetGridPosition(), 5f) > 50;
                return hasEnoughSurroundingNature;
            })
            .ToList();

        var candidatesCount = candidates.Count;
        if (candidatesCount > 0)
        {
            var houseToUpgrade = candidates[Random.Range(0, candidatesCount)];
            houseToUpgrade.GetOccupantHouse().Upgrade();

            _lastUpgradedBigHouse = Time.fixedTime;
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