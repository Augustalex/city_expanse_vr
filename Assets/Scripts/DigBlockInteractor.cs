using UnityEngine;

public class DigBlockInteractor : BlockInteractor
{
    private void OnTriggerEnter(Collider other)
    {
        Interact(other.gameObject);
    }

    public override void Interact(GameObject other)
    {
        if (!IsActivated()) return;

        var blockComponent = other.gameObject.GetComponent<Block>();

        if (blockComponent && blockComponent.blockType == Block.BlockType.Grass && blockComponent.IsGroundLevel() && blockComponent.IsVacant())
        {
            GetWorldPlane().RemoveAndDestroyBlock(blockComponent);
            PlaySound(BlockSoundLibrary.BlockSound.Dig);
        }
    }
}