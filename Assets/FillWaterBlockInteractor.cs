using UnityEngine;

public class FillWaterBlockInteractor : BlockInteractor
{
    public GameObject waterBlockTemplate;

    private void OnTriggerEnter(Collider other)
    {
        if (!IsActivated()) return;

        var blockComponent = other.gameObject.GetComponent<Block>();
        if (blockComponent && blockComponent.IsInteractable() && blockComponent.Vacant() && blockComponent.IsGroundLevel())
        {
            var waterBlock = Instantiate(waterBlockTemplate);
            blockComponent.TurnOverSpotTo(waterBlock);
            waterBlock.GetComponent<Block>().ShortFreeze();
            PlayGeneralSound();
        }
    }
}