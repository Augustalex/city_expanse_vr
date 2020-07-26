using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaiseWaterBlockInteractor : BlockInteractor
{
    public GameObject waterBlockTemplate;

    private void OnTriggerEnter(Collider other)
    {
        if (!IsActivated()) return;

        var blockComponent = other.gameObject.GetComponent<Block>();
        if (blockComponent && blockComponent.IsInteractable() && blockComponent.Vacant())
        {
            var waterBlock = Instantiate(waterBlockTemplate);
            blockComponent.PlaceOnTopOfSelf(waterBlock);
            waterBlock.GetComponent<Block>().PermanentFreeze();
            PlayGeneralSound();
        }
    }
}
