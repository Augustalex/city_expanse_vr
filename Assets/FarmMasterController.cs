using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FarmMasterController : MonoBehaviour
{
    public GameObject farmControllerTemplate;
    
    public Block SetupFarmControllerForBlock(Block block)
    {
        var farmControllerHolder = Instantiate(Get().farmControllerTemplate);
        var farmController = farmControllerHolder.GetComponent<FarmController>();
        
        return farmController.MakeToSoilWithFarm(block);
    }

    public static FarmMasterController Get()
    {
        return FindObjectOfType<FarmMasterController>();
    }

    public int CountFarms()
    {
        return WorldPlane.Get()
            .blocksRepository
            .StreamBlocks()
            .Count(block => block.GetComponent<FarmSpawn>() != null);
    }
}