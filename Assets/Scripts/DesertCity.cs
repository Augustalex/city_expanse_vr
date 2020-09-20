using System;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class DesertCity : MonoBehaviour
{
    public GameObject tinyHouseTemplate;
    
    private WorldPlane _worldPlane;
    private double _lastPlacedHouse;
    private bool _sandSpawned;

    void Start()
    {
        _worldPlane = GetComponent<WorldPlane>();
        _lastPlacedHouse = Time.fixedTime - 10;
    }

    void Update()
    {
        if (Random.value < .1f) return;

        if (_worldPlane.CountBlocksOfType(Block.BlockType.Sand) > 0)
        {
            _sandSpawned = true;
        }

        if (_sandSpawned)
        {
            var delta = Time.fixedTime - _lastPlacedHouse;
            if (delta > 10f)
            {
                if (CanSpawnAnotherHouse())
                {
                    SpawnOneHouse();
                }
            }
        }
    }

    private bool CanSpawnAnotherHouse()
    {
        var houses = _worldPlane.GetBlocksWithDesertHouses();
        return houses.Count() < 5;
    }

    private void SpawnOneHouse()
    {
        var candidates = _worldPlane.GetSandBlocks()
            .OrderBy(_ => Random.value)
            .SelectMany(sandBlock =>
                _worldPlane.GetNearbyBlocks(sandBlock.GetGridPosition())
                    .Where(block => block.IsWater())
                    .Select(block => new Tuple<Block, Block>(sandBlock, block))
            )
            .ToList();

        var candidatesCount = candidates.Count;
        if (candidatesCount > 0)
        {
            var (sandBlock, lookAtTarget) = candidates[Random.Range(0, candidatesCount)];
            var house = Instantiate(tinyHouseTemplate);
            sandBlock.Occupy(house);
            
            var target = lookAtTarget.transform.position;
            target.y = house.transform.position.y;
            house.transform.LookAt(target);

            _lastPlacedHouse = Time.fixedTime;
        }
    }
}