using UnityEngine;

public class DigBlockInteractor : BlockInteractor
{
    public override bool LockOnLayer()
    {
        return true;
    }

    public override void Interact(GameObject other)
    {
        var blockComponent = other.gameObject.GetComponent<Block>();
        GetWorldPlane().RemoveAndDestroyBlock(blockComponent);
        PlaySound(BlockSoundLibrary.BlockSound.Dig);
    }

    public override bool Interactable(GameObject other)
    {
        if (!IsActivated()) return false;

        var blockComponent = other.gameObject.GetComponent<Block>();

        return CanDig(blockComponent);
    }

    private bool CanDig(Block blockComponent)
    {
        if (FeatureToggles.Get().digAnywhere)
        {
            return blockComponent
                   && blockComponent.blockType == Block.BlockType.Grass
                   && blockComponent.IsTopBlockInStack()
                   && blockComponent.IsVacant();
        }
        else
        {
            return blockComponent
                   && blockComponent.blockType == Block.BlockType.Grass
                   && blockComponent.IsGroundLevel()
                   && blockComponent.IsVacant();
        }
    }
}