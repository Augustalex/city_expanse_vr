using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MakeDesertInteractor : BlockInteractor
{
    public GameObject sandBlockTemplate;

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
            var sand = Instantiate(sandBlockTemplate);
            var sandBlock = sand.GetComponentInChildren<Block>();
            GetWorldPlane().ReplaceBlock(blockComponent, sandBlock);
            sandBlock.ShortFreeze();
            PlayGeneralSound();
        }   
    }
}