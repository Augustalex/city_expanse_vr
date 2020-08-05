using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaiseWaterBlockInteractor : BlockInteractor
{
    public GameObject waterBlockTemplate;

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
            var water = Instantiate(waterBlockTemplate);
            var waterBlock = water.GetComponentInChildren<Block>();
            GetWorldPlane().AddBlockOnTopOf(waterBlock, water, blockComponent);
            waterBlock.ShortFreeze();
            PlayGeneralSound();
        }
    }
}
