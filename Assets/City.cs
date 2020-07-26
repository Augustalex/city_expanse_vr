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
        if (delta > 8 && Random.value < .1)
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