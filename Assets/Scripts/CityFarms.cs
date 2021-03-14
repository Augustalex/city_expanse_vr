using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(City))]
public class CityFarms : MonoBehaviour
{
    private WorldPlane _worldPlane;
    private bool _placedFirstFarm;

    void Start()
    {
        _worldPlane = WorldPlane.Get();
    }

    void Update()
    {
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
        var thereAreSomeVacantBlock = false;
        var houseBlock = houses.OrderBy(_ => Random.value).First();
        var vacantLots = _worldPlane.GetNearbyVacantLotsStream(houseBlock.GetGridPosition())
            .OrderBy(_ =>
            {
                thereAreSomeVacantBlock = true;
                return Random.value;
            });

        if (thereAreSomeVacantBlock)
        {
            var lotToReplace = vacantLots.First();
            FarmMasterController.Get().SetupFarmControllerForBlock(lotToReplace);

            _placedFirstFarm = true;
        }
    }
}