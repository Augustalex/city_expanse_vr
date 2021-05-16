using System;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class ConstructDocksBlockInteractor : BlockInteractor
{
    public override InteractorHolder.BlockInteractors InteractorType => InteractorHolder.BlockInteractors.ConstructDocks;

    public GameObject docksTemplate;
    
    new void Start()
    {
        base.Start();

        showInteractionGhost = true;
    }

    public override bool Interactable(GameObject other)
    {
        if (!IsActivated()) return false;

        var blockComponent = other.gameObject.GetComponent<Block>();

        return blockComponent &&
               blockComponent.IsLot() &&
               blockComponent.IsVacant() &&
               blockComponent.IsInteractable() &&
               CityDocks.Get().CanBuildADock() &&
               GetCandidate(other) != null;
    }

    public override bool LockOnLayer()
    {
        return true;
    }

    public override void Interact(GameObject other)
    {
        var vacantLot = other.gameObject.GetComponent<Block>();

        var candidate = GetCandidate(other);
        if (candidate)
        {
            var dockSpawn = Instantiate(docksTemplate);
            vacantLot.Occupy(dockSpawn);
            var target = candidate.transform.position;
            target.y = dockSpawn.transform.position.y;
            dockSpawn.transform.LookAt(target);

            PlaySound(BlockSoundLibrary.BlockSound.RaiseLand);
        }
    }

    private Block GetCandidate(GameObject other)
    {
        var worldPlane = GetWorldPlane();
        var vacantLot = other.gameObject.GetComponent<Block>();

        var candidates = GetWorldPlane()
            .GetNearbyBlocks(vacantLot.GetGridPosition())
            .Where(waterBlock =>
                waterBlock.IsGroundLevel() &&
                worldPlane.GetMajorityBlockTypeWithinRange(waterBlock.GetGridPosition(), 1f)
                == Block.BlockType.Lake
                && worldPlane.NoNearby(waterBlock.GetGridPosition(), 3f, CityDocks.BlockHasDocksSpawn))
            .Where(otherBlock => otherBlock.IsWater() && otherBlock.IsOnSameHeightAs(vacantLot))
            .ToList();

        var candidatesCount = candidates.Count;
        if (candidatesCount > 0)
        {
            return candidates[Random.Range(0, candidatesCount)];
        }
        else
        {
            return null;
        }
    }
}