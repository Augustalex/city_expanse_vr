using System;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(City))]
public class CityWoodcutters : MonoBehaviour
{
    public GameObject woodcutterSpawnTemplate;
    private WorldPlane _worldPlane;
    private bool _placedFirstFarm;

    void Start()
    {
        _worldPlane = WorldPlane.Get();
    }

    void Update()
    {
        if (Random.value < .01f)
        {
            var houseCount = _worldPlane.GetBlocksWithHouses().Count;
            if (houseCount < 3) return;

            var woodcutters = _worldPlane.GetBlocksWithWoodcutters().Count();
            var woodcutterToHouseRatio = (float)Mathf.Max(woodcutters, 1) / (float)houseCount;
            if (woodcutterToHouseRatio > .25f) return;
            
            var greensAndVacantLot = _worldPlane
                .GetBlocksWithGreens()
                .SelectMany(block =>
                {
                    return _worldPlane
                        .GetNearbyVacantLots(block.GetGridPosition())
                        .Select(vacantLot => new Tuple<Block, Block>(block, vacantLot));
                })
                .OrderBy(_ => Random.value)
                .FirstOrDefault();

            if (greensAndVacantLot != null)
            {
                SpawnWoodcutter(greensAndVacantLot);
            }
        }
    }

    private void SpawnWoodcutter(Tuple<Block, Block> greensAndVacantLot)
    {
        var (greensBlock, vacantLot) = greensAndVacantLot;

        var woodcutterBlock = Instantiate(woodcutterSpawnTemplate);
        vacantLot.Occupy(woodcutterBlock);
        var target = greensBlock.transform.position;
        target.y = woodcutterBlock.transform.position.y;
        woodcutterBlock.transform.LookAt(target);
    }
}