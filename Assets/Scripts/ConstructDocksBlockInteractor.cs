using System;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class ConstructDocksBlockInteractor : BlockInteractor
{
    public GameObject docksTemplate;

    public override bool Interactable(GameObject other)
    {
        if (!IsActivated()) return false;

        var blockComponent = other.gameObject.GetComponent<Block>();

        return blockComponent &&
               blockComponent.IsLot() &&
               blockComponent.IsVacant() &&
               blockComponent.IsInteractable() &&
               CityDocks.Get().CanBuildADock();
    }

    public override bool LockOnLayer()
    {
        return true;
    }

    public override void Interact(GameObject other)
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
            var waterBlock = candidates[Random.Range(0, candidatesCount)];
            var dockSpawn = Instantiate(docksTemplate);
            vacantLot.Occupy(dockSpawn);
            var target = waterBlock.transform.position;
            target.y = dockSpawn.transform.position.y;
            dockSpawn.transform.LookAt(target);

            PlaySound(BlockSoundLibrary.BlockSound.RaiseLand);
        }
    }
}