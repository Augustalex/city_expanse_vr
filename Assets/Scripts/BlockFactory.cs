using UnityEngine;

public class BlockFactory : MonoBehaviour
{
    public GameObject grassBlockTemplate;
    public GameObject topWaterBlockTemplate;
    public GameObject sandBlockTemplate;
    public GameObject oceanShallowBlockTemplate;
    public GameObject oceanFullHeightBlockTemplate;
    
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
}