using blockInteractions;
using UnityEngine;

public class RaiseWaterBlockInteractor : BlockInteractor
{
    private void OnTriggerEnter(Collider other)
    {
        Interact(other.gameObject);
    }

    public override void Interact(GameObject other)
    {
        if (!IsActivated()) return;

        var blockComponent = other.gameObject.GetComponent<Block>();
        if (blockComponent && blockComponent.IsInteractable() && blockComponent.IsVacant())
        {
            RaiseWater.Get().Use(blockComponent);
            PlayGeneralSound();
        }
    }
}
