using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(City))]
public class CityWoodcutters : MonoBehaviour
{
    public GameObject woodcutterSpawnTemplate;
    public GameObject stockpileTemplate;
    public GameObject woodTemplate;
    
    private WorldPlane _worldPlane;
    private bool _placedFirstFarm;
    private bool _hasUnfulfilledRequestToStoreWood;
    private static CityWoodcutters _cityWoodcuttersInstance;

    private void Awake()
    {
        _cityWoodcuttersInstance = this;
    }

    public static CityWoodcutters Get()
    {
        return _cityWoodcuttersInstance;
    }
    
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
            if (woodcutterToHouseRatio > .1f) return;
            
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

        if (_hasUnfulfilledRequestToStoreWood)
        {
            _hasUnfulfilledRequestToStoreWood = false;

            SpawnStockpile();
        }
    }

    public void RequestStorageSpace()
    {
        var hasSpace = GetStockpilesWithStorageAvailable().Any();
        if (!hasSpace)
        {
            _hasUnfulfilledRequestToStoreWood = true;
        }
    }

    public void StoreWood()
    {
        var stockpile = GetStockpilesWithStorageAvailable().FirstOrDefault();
        if (stockpile != null) stockpile.Store(woodTemplate);
    }

    public bool RequireWood(int amount)
    {
        return GetWood() >= amount;
    }

    public void ConsumeWood(int amount)
    {
        var stockpilesWithWood = _worldPlane.GetBlocksWithOccupants()
            .Select(block => block.GetOccupant().GetComponent<StockpileSpawn>())
            .Where(component => component != null && component.HasWood());

        int remainingAmount = amount;
        foreach (var stockpile in stockpilesWithWood)
        {
            var toConsume = Mathf.Min(remainingAmount, stockpile.StoredCount());
            remainingAmount -= toConsume;
            
            stockpile.Consume(toConsume);

            if (remainingAmount == 0) return;
        }
    }
    
    public IEnumerable<StockpileSpawn> GetStockpilesWithStorageAvailable()
    {
        return _worldPlane.GetBlocksWithOccupants()
            .Select(block => block.GetOccupant().GetComponent<StockpileSpawn>())
            .Where(component => component != null && component.CanStoreMore());
    }

    public int GetWood()
    {
        return _worldPlane.GetBlocksWithOccupants()
            .Select(block => block.GetOccupant().GetComponent<StockpileSpawn>())
            .Where(component => component != null && component.HasWood())
            .Sum(component => component.StoredCount());
    }

    private void SpawnStockpile()
    {
        var vacantLot = _worldPlane
            .GetBlocksWithWoodcutters()
            .SelectMany(block => _worldPlane.GetNearbyVacantLots(block.GetGridPosition()))
            .OrderBy(_ => Random.value)
            .FirstOrDefault();

        if (vacantLot != null)
        {
            vacantLot.CreateAndOccupy(stockpileTemplate);
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