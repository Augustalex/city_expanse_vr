using System.Linq;
using UnityEngine;

public class FarmMasterController : MonoBehaviour
{
    public GameObject farmControllerTemplate;
    private static FarmMasterController _farmMasterControllerInstance;

    private int _farmCount = 0;

    public static void OnResetWorld()
    {
        Get().ClearFarmCount();
    }

    private void ClearFarmCount()
    {
        _farmCount = 0;
    }

    public void IncreaseFarmCount(int count)
    {
        _farmCount += count;
    }

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
        return _farmCount;
    }
}