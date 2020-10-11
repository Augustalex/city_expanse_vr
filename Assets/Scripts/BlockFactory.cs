using UnityEngine;

public class BlockFactory : MonoBehaviour
{
    public GameObject grassBlockTemplate;
    public GameObject topWaterBlockTemplate;
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