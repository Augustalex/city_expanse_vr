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
            .Count(block =>
            {
                if (block == null) return false;
                if (!block.HasOccupant()) return false;
                
                var occupant = block.GetOccupant();
                var farmSpawn = occupant.GetComponent<FarmSpawn>();
                if (farmSpawn == null) return false;

                return farmSpawn.IsGrown();
            });
    }
}