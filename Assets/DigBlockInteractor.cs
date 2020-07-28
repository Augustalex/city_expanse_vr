using UnityEngine;

public class DigBlockInteractor : BlockInteractor
{
    private void OnTriggerEnter(Collider other)
    {
        if (!IsActivated()) return;

        var blockComponent = other.gameObject.GetComponent<Block>();

        if (blockComponent && blockComponent.blockType == Block.BlockType.Grass && blockComponent.IsGroundLevel() && blockComponent.IsVacant())
        {
            blockComponent.DestroySelf();
            PlayGeneralSound();
        }
    }
}