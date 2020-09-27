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

        if (CanDig(blockComponent))
        {
            GetWorldPlane().RemoveAndDestroyBlock(blockComponent);
            PlaySound(BlockSoundLibrary.BlockSound.Dig);
        }
    }

    private bool CanDig(Block blockComponent)
    {
        if (FeatureToggles.Get().digAnywhere)
        {
            return blockComponent 
                   && blockComponent.blockType == Block.BlockType.Grass
                   && blockComponent.IsTopLevel()
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