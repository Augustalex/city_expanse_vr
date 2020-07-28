using UnityEngine;

public class FillWaterBlockInteractor : BlockInteractor
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
        if (blockComponent && blockComponent.IsInteractable() && blockComponent.IsVacant() && blockComponent.IsGroundLevel())
        {
            var water = Instantiate(waterBlockTemplate);
            var waterBlock = water.GetComponentInChildren<Block>();
            blockComponent.TurnOverSpotTo(waterBlock);
            waterBlock.ShortFreeze();
            PlayGeneralSound();
        }   
    }
}