using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(City))]
public class CityFarms : MonoBehaviour
{
    private WorldPlane _worldPlane;
    private bool _placedFirstFarm;
    private FeatureToggles _featureToggles;

    private static CityFarms _instance;

    public static CityFarms Get()
    {
        return _instance;
    }

    private void Awake()
    {
        _instance = this;
    }

    void Start()
    {
        _worldPlane = WorldPlane.Get();
        _featureToggles = FeatureToggles.Get();
    }

    void Update()
    {
        if (!_featureToggles.farmsSpawn) return;
        
        if (!_placedFirstFarm && Random.value < .01f)
        {
            var houses = _worldPlane.GetBlocksWithHousesNotNearWater().ToList();

            if (houses.Count > 0)
            {
                SpawnMasterFarm(houses);
            }
            else
            {
                var bigHouses = _worldPlane.GetBlocksWithHouses()
                    .Where(block => block.GetOccupantHouse().IsInnerCityHouse())
                    .ToList();
                if (bigHouses.Count > 0)
                {
                    SpawnMasterFarm(bigHouses);
                }
            }
        }
    }

    private void SpawnMasterFarm(List<Block> houses)
    {
        var houseBlock = houses.OrderBy(_ => Random.value).First();
        var lotToReplace = _worldPlane.GetNearbyVacantLotsStream(houseBlock.GetGridPosition())
            .OrderBy(_ => Random.value)
            .FirstOrDefault();

        if (lotToReplace != null)
        {
            FarmMasterController.Get().SetupFarmControllerForBlock(lotToReplace);

            _placedFirstFarm = true;
        }
    }

    public bool CanManuallyConstructAnyKindOfFarm()
    {
        // TODO: Make Farms grow slowly over time. That why you can take your time with the game and not just speed build.
        float houseCount = _worldPlane.GetBlocksWithHousesStream().Count();
        return houseCount >= City.SelfSustainedHouses;
    }
}