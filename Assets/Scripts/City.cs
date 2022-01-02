﻿using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

[RequireComponent(typeof(WorldPlane))]
public class City : MonoBehaviour
{
    public GameObject[] grassPuzzleTemplates;
    public GameObject housePuzzleTemplate;
    public GameObject tinyHouseTemplate;
    public GameObject sandBlockTemplate;

    public static int SelfSustainedHouses = 3;

    private WorldPlane _worldPlane;
    private double _lastPlacedHouse;
    private bool _sandSpawned;
    private SandSpreadController _sandSpreadController;
    private float _lastPlacedInnerCityHouse;
    private int _ticket = -1;
    private WorkQueue _workQueue;
    private FeatureToggles _featureToggles;
    private static City _instance;

    private bool _placedFirstHouse = false;
    private bool _firstBigHouse = false;

    private void Awake()
    {
        _instance = this;
    }

    public static City Get()
    {
        return _instance;
    }

    void Start()
    {
        _worldPlane = GetComponent<WorldPlane>();
        _lastPlacedHouse = Time.fixedTime - 10;
        _sandSpreadController = SandSpreadController.Get();
        _featureToggles = FeatureToggles.Get();

        _workQueue = WorkQueue.Get();
    }

    void Update()
    {
        if (!_worldPlane.WorldGenerationDone()) return;
        if (!CanWorkThisFrame()) return;

        if (BuildingSpawn.ActiveSpawnCount() < 3)
        {
            if (_featureToggles.houseSpawn)
            {
                // var delta = Time.fixedTime - _lastPlacedHouse;
                // if (delta > 5f && Random.value < .1f)
                // {
                //     if (CanSpawnAnotherHouse())
                //     {
                //         SpawnOneHouse();
                //     }
                // }

                SpawnOneHouse();

                // var innerCityDelta = Time.fixedTime - _lastPlacedInnerCityHouse;
                // if (innerCityDelta > 10f && Random.value < .2f)
                // {
                //     if (Random.value < .1f)
                //     {
                //         SpawnInnerCityHouse();
                //     }
                // }
            }

            // if (Random.value < .01f && HasNoOtherBigHouses())
            // {
            //     SpawnBigHouse();
            // }
        }

        if (!_featureToggles.desertsAreBeaches)
        {
            if (!_sandSpawned
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
    }

    private bool CanWorkThisFrame()
    {
        if (_workQueue.HasExpiredTicket(_ticket))
        {
            _ticket = _workQueue.GetTicket();
        }

        return _workQueue.HasTicketForFrame(_ticket);
    }

    private bool HasNoOtherBigHouses()
    {
        var amountOfBigHouses = _worldPlane.GetBlocksWithHouses()
            .Count(block => block.GetOccupantHouse().IsBig());

        return amountOfBigHouses < 2;
    }

    public bool CanSpawnAnotherHouse()
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
            .GetVacantBlocksStream()
            .Where(vacantBlock => _worldPlane.BlockCanBeReplacedBySandBlock(vacantBlock))
            .OrderBy(_ => Random.value)
            .First();

        var sandBlock = sandBlockRoot.GetComponentInChildren<Block>();
        _worldPlane.ReplaceBlock(block, sandBlock);
    }

    public void SpawnOneHouse()
    {
        var allSpawns = FindObjectsOfType<PuzzleSpawn>();
        var vacantLots = _worldPlane
            .GetAllTopLots()
            .Where(pair => pair.Value.IsVacant() && pair.Value.IsGrass() && allSpawns.All(spawn => spawn.spawnGridPosition != pair.Key))
            .Select(pair => pair.Value)
            .ToList();

        if (vacantLots.Count == 0) return;
        
        var vacantLot = vacantLots[Random.Range(0, vacantLots.Count)];

        if (vacantLot != null)
        {
            var template = grassPuzzleTemplates[Random.Range(0, grassPuzzleTemplates.Length)];
            var puzzleSpawn = Instantiate(template);
            var puzzleSpawnComponent = puzzleSpawn.GetComponentInChildren<PuzzleSpawn>();
            puzzleSpawnComponent.spawnGridPosition = vacantLot._gridPosition;
            _lastPlacedHouse = Time.fixedTime;
        }
    }

    private void SpawnInnerCityHouse()
    {
        var candidates = new List<Tuple<Block, Block>>();
        foreach (var bigHouse in _worldPlane.GetBlocksWithHousesStream())
        {
            if (!bigHouse.GetOccupantHouse().IsBig()) continue;

            foreach (var nearbyVacantLot in _worldPlane.GetNearbyVacantLotsStream(bigHouse.GetGridPosition()))
            {
                candidates.Add(new Tuple<Block, Block>(bigHouse, nearbyVacantLot));
            }
        }

        if (candidates.Count > 0)
        {
            var (bigHouse, vacantLot) = candidates.ElementAt(Random.Range(0, candidates.Count));
            var target = bigHouse.transform.position;
            var buildingSpawn = BlockFactory.Get().BuildingSpawn(vacantLot, target);
            vacantLot.Occupy(buildingSpawn);
            buildingSpawn.GetComponent<BuildingSpawn>().GroundHighlight(vacantLot);

            var buildingSpawnComponent = buildingSpawn.GetComponent<BuildingSpawn>();
            buildingSpawnComponent.CreateBuildingAction = () =>
            {
                var house = Instantiate(tinyHouseTemplate);
                house.GetComponent<HouseSpawn>().SetIsInnerCity();

                return house.gameObject;
            };
            buildingSpawnComponent.GetBuildingInfo = () => new BuildingInfo {devotees = 10};

            _lastPlacedInnerCityHouse = Time.fixedTime;
        }
    }

    private void SpawnBigHouse()
    {
        List<Block> candidates = null;

        foreach (var block in _worldPlane
            .GetBlocksWithHousesStream()
            .OrderBy(_ => Random.value)
            .Take(2))
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
                if (candidates == null)
                {
                    candidates = new List<Block>();
                }

                candidates.Add(block);
            }
        }

        if (candidates != null)
        {
            var houseToUpgrade = candidates[Random.Range(0, candidates.Count)];
            houseToUpgrade.GetOccupantHouse().Upgrade();
            _lastPlacedHouse = Time.fixedTime;

            if (!_firstBigHouse)
            {
                _firstBigHouse = true;
                DiscoveryManager.Get().RegisterNewDiscover(DiscoveryManager.Discoverable.BigHouse);
            }
        }
    }
}