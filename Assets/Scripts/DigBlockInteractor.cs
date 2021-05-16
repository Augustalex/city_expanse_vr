using UnityEngine;

public class DigBlockInteractor : BlockInteractor
{
    public override InteractorHolder.BlockInteractors InteractorType => InteractorHolder.BlockInteractors.Dig;

    public override bool LockOnLayer()
    {
        return true;
    }

    public override void Interact(GameObject other)
    {
        var blockComponent = other.GetComponent<Block>();
        GetWorldPlane().RemoveAndDestroyBlock(blockComponent);
        PlaySound(BlockSoundLibrary.BlockSound.Dig, other.transform.position);
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
                   && blockComponent.CanBeDugAsAnIndependentBlock()
                   && blockComponent.IsTopBlockInStack()
                   && blockComponent.IsVacant()
                   && !blockComponent.IsLowestLevel();
        }
        else
        {
            return blockComponent
                   && blockComponent.CanBeDugAsAnIndependentBlock()
                   && blockComponent.IsGroundLevel()
                   && blockComponent.IsVacant();
        }
    }
}