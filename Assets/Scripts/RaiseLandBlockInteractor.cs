using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class RaiseLandBlockInteractor : BlockInteractor
{
    public override InteractorHolder.BlockInteractors InteractorType => InteractorHolder.BlockInteractors.RaiseLand;

    public GameObject grassBlockTemplate;

    public override bool Interactable(GameObject other)
    {
        if (!IsActivated()) return false;

        var blockComponent = other.gameObject.GetComponent<Block>();

        return blockComponent &&
               blockComponent.blockType == Block.BlockType.Grass &&
               (blockComponent.OccupiedByGreens() || blockComponent.IsVacant()) &&
               blockComponent.IsInteractable();
    }

    public override bool LockOnLayer()
    {
        return true;
    }

    public override void Interact(GameObject other)
    {
        var blockComponent = other.gameObject.GetComponent<Block>();
        blockComponent.DestroyOccupant();
        
        var grass = Instantiate(grassBlockTemplate);
        var grassBlock = grass.GetComponentInChildren<Block>();
        GetWorldPlane().AddBlockOnTopOf(grassBlock, grass, blockComponent);
        PlaySound(BlockSoundLibrary.BlockSound.RaiseLand, other.transform.position);
    }
}