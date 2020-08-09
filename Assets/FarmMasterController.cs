using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FarmMasterController : MonoBehaviour
{
    public GameObject farmControllerTemplate;
    private static FarmMasterController _farmMasterControllerInstance;

    private void Awake()
    {
        _farmMasterControllerInstance = this;
    }

    public Block SetupFarmControllerForBlock(Block block)
    {
        var farmControllerHolder = Instantiate(Get().farmControllerTemplate);
        var farmController = farmControllerHolder.GetComponent<FarmController>();
        
        return farmController.MakeToSoilWithFarm(block);
    }

    public static FarmMasterController Get()
    {
        return _farmMasterControllerInstance;
    }

    public int CountFarms()
    {
        return WorldPlane.Get()
            .blocksRepository
            .StreamBlocks()
            .Count(block => (block.GetOccupant()?.GetComponent<FarmSpawn>()?.IsGrown()) == true);
    }
}