using UnityEngine;

public class BlockFactory : MonoBehaviour
{
    public GameObject grassBlockTemplate;
    public GameObject topWaterBlockTemplate;
    public GameObject regularWaterBlockTemplate; // Top side - but not "outside water"
    public GameObject sandBlockTemplate;
    public GameObject oceanShallowBlockTemplate;
    public GameObject oceanFullHeightBlockTemplate;
    
    public GameObject interactableGhostTemplate;
    public GameObject nonInteractableGhostTemplate;

    public GameObject buildingSpawnTemplate;
    public GameObject tinyHouseTemplate;

    private static BlockFactory _blockFactoryInstance;

    void Awake()
    {
        _blockFactoryInstance = this;
    }
    
    public static BlockFactory Get()
    {
        return _blockFactoryInstance;
    }

    public GameObject GrassBlock()
    {
        return Instantiate(grassBlockTemplate);
    }
    
    public GameObject TinyHouse()
    {
        return Instantiate(tinyHouseTemplate);
    }

    public GameObject BuildingSpawn(Block vacantLot, Vector3 lookingTarget)
    {
        var buildingSpawn = Instantiate(buildingSpawnTemplate);
        var buildingSpawnComponent = buildingSpawn.GetComponent<BuildingSpawn>();
        buildingSpawnComponent.spawnLot = vacantLot;
        buildingSpawnComponent.spawnLookingTarget = lookingTarget;

        return buildingSpawn;
    }
}