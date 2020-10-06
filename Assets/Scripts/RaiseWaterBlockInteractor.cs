using blockInteractions;
using UnityEngine;

public class RaiseWaterBlockInteractor : BlockInteractor
{
    public override bool Interactable(GameObject other)
    {
        if (!IsActivated()) return false;

        var blockComponent = other.gameObject.GetComponent<Block>();
        return blockComponent && blockComponent.IsInteractable() && blockComponent.IsVacant();
    }

    public override bool LockOnLayer()
    {
        return true;
    }

    public override void Interact(GameObject other)
    {
        var blockComponent = other.gameObject.GetComponent<Block>();

        RaiseWater.Get().Use(blockComponent);
        PlaySound(BlockSoundLibrary.BlockSound.PlaceItem);
    }
}