using UnityEngine;

[RequireComponent(typeof(BlockRelative))]
public class StockpileSpawn : MonoBehaviour
{
    private int _stored;
    private WorldPlane _worldPlane;
    private Block _block;
    private const int MaxStorage = 6;

    void Start()
    {
        _worldPlane = WorldPlane.Get();
        _block = GetComponent<BlockRelative>().block;
    }

    public void Store(GameObject objectTemplate)
    {
        var storeObject = Instantiate(objectTemplate, transform, false);
        storeObject.transform.position = transform.position + Vector3.up * .1f;
        _stored += 1;
    }
    
    public bool CanStoreMore()
    {
        return _stored <= MaxStorage;
    }

    public bool HasWood()
    {
        return _stored > 0;
    }

    public int StoredCount()
    {
        return _stored;
    }

    public void Consume(int amount)
    {
        _stored -= amount;
    }
}
