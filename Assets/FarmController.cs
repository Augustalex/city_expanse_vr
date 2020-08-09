using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using Random = UnityEngine.Random;

public class FarmController : MonoBehaviour
{
    public GameObject soilTemplate;
    public GameObject farmSpawnTemplate;
    public GameObject siloSpawnTemplate;

    private List<Block> _soils = new List<Block>();
    private WorldPlane _worldPlane;
    private bool _siloBuilt;
    private const int MaxFarmCount = 15;

    private void Awake()
    {
        _worldPlane = WorldPlane.Get();
    }

    public Block MakeToSoilWithFarm(Block block)
    {
        var farmSpawn = Instantiate(farmSpawnTemplate, null, false);
        return MakeToSoilWith(block, farmSpawn);
    }

    public Block MakeToSoilWith(Block block, GameObject blockToBeOccupant)
    {
        var soilRoot = Instantiate(soilTemplate);
        var soilBlock = soilRoot.GetComponentInChildren<Block>();

        _worldPlane.ReplaceBlock(block, soilBlock);

        soilBlock.Occupy(blockToBeOccupant);

        soilBlock.BeforeDestroy += () =>
        {
            _soils.Remove(soilBlock);
            if (!_soils.Any())
            {
                Destroy(gameObject);
            }
        };
        _soils.Add(soilBlock);

        return soilBlock;
    }

    private void Update()
    {
        if (Random.value < 0.005f)
        {
            foreach (var soil in _soils)
            {
                var farmSpawn = soil.GetOccupant().GetComponent<FarmSpawn>();
                if (farmSpawn)
                {
                    if (FeatureToggles.Get().farmsGrowAtRandom)
                    {
                        farmSpawn.Grow();
                    }
                }
            }
        }

        Debug.Log("_soils.Count < MaxFarmCount * .6f: " + (_soils.Count < MaxFarmCount * .6f));
        if (_soils.Count < MaxFarmCount * .6f || (_siloBuilt && _soils.Count <= MaxFarmCount))
        {
            if (Random.value < .001f)
            {
                Expand();
            }
        }
        else if (!_siloBuilt && Random.value < .001f)
        {
            BuildSilo();
        }
    }

    private void Expand()
    {
        foreach (var block in _soils.ToList())
        {
            var candidates = _worldPlane.GetNearbyVacantLots(block.GetGridPosition())
                .Where(vacantBlock =>
                {
                    var amountOfNeighbouringWaterBlocks = _worldPlane.GetNearbyBlocks(vacantBlock.GetGridPosition())
                        .Count(nearbyBlock => nearbyBlock.IsWater());
                    return amountOfNeighbouringWaterBlocks == 0;
                })
                .OrderBy(_ => Random.value)
                .ToList();

            if (candidates.Any())
            {
                var choice = candidates.First();
                MakeToSoilWithFarm(choice);
                
                return;
            }
        }
    }

    private void BuildSilo()
    {
        foreach (var block in _soils.ToList())
        {
            var candidates = _worldPlane.GetNearbyVacantLots(block.GetGridPosition())
                .OrderBy(_ => Random.value)
                .ToList();

            if (candidates.Any())
            {
                var choice = candidates.First();
                var siloSpawn = Instantiate(siloSpawnTemplate, null, false);
                MakeToSoilWith(choice, siloSpawn);
                _siloBuilt = true;

                return;
            }
        }
    }
}