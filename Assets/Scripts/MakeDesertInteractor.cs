using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MakeDesertInteractor : BlockInteractor
{
    public GameObject sandBlockTemplate;

    public override bool Interactable(GameObject other)
    {
        if (!IsActivated()) return false;

        var blockComponent = other.gameObject.GetComponent<Block>();
        return blockComponent
               && blockComponent.IsInteractable()
               && blockComponent.IsVacant()
               && blockComponent.IsGroundLevel()
               && !blockComponent.IsOutsideWater();
    }

    public override bool LockOnLayer()
    {
        return true;
    }

    public override void Interact(GameObject other)
    {
        var blockComponent = other.gameObject.GetComponent<Block>();

        var sand = Instantiate(sandBlockTemplate);
        var sandBlock = sand.GetComponentInChildren<Block>();
        GetWorldPlane().ReplaceBlock(blockComponent, sandBlock);
        sandBlock.ShortFreeze();
        PlaySound(BlockSoundLibrary.BlockSound.Basic);
    }
}