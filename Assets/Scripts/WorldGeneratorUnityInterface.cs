using UnityEngine;

public interface IWorldGeneratorUnityInterface
{
    GameObject Create(GameObject blockToUse);
    void AddAndPositionBlock(Block block, Vector3 blockPosition);
    void AddBlockOnTopOf(Block block, GameObject blockObject, Block previousBlock);
    Block GetBlockAtTopOfStack(Vector3 vector3);
}