using System;
using UnityEngine;

public class WorldPlaneWorldGeneratorInterface : MonoBehaviour, IWorldGeneratorUnityInterface
{
    private WorldPlane _worldPlane;
    private static WorldPlaneWorldGeneratorInterface _instance;

    private void Awake()
    {
        _instance = this;
    }

    private void Start()
    {
        _worldPlane = WorldPlane.Get();
    }

    public GameObject Create(GameObject blockToUse)
    {
        return Instantiate(blockToUse);
    }

    public void AddAndPositionBlock(Block block, Vector3 blockPosition)
    {
        _worldPlane.AddAndPositionBlock(block, blockPosition);   
    }

    public void AddBlockOnTopOf(Block block, GameObject blockObject, Block previousBlock)
    {
        _worldPlane.AddBlockOnTopOf(block, blockObject, previousBlock);
    }

    public Block GetBlockAtTopOfStack(Vector3 vector3)
    {
        return _worldPlane.GetBlockAtTopOfStack(vector3);
    }

    public static WorldPlaneWorldGeneratorInterface Get()
    {
        return _instance;
    }
}