using System;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class ConstructFarmBlockInteractor : BlockInteractor
{
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
               IsCandidate(other);
    }

    public override bool LockOnLayer()
    {
        return true;
    }

    public override void Interact(GameObject other)
    {
        var vacantLot = other.gameObject.GetComponent<Block>();

        var nearbyFarmController = GetClosestFarmController(vacantLot.GetGridPosition());
        nearbyFarmController.AddBlockToSoilNetwork(vacantLot);
    }

    private bool IsCandidate(GameObject other)
    {
        var vacantLot = other.gameObject.GetComponent<Block>();

        var amountOfNeighbouringWaterBlocks = GetWorldPlane().GetNearbyBlocks(vacantLot.GetGridPosition())
            .Count(nearbyBlock => nearbyBlock.IsWater());
        if (amountOfNeighbouringWaterBlocks > 0) return false;

        var nearbyFarmController = GetClosestFarmController(vacantLot.GetGridPosition());
        return nearbyFarmController != null;
    }

    private FarmController GetClosestFarmController(Vector3 gridPosition)
    {
        foreach (var nearbyBlock in GetWorldPlane().GetNearbyBlocks(gridPosition))
        {
            if (!nearbyBlock.IsSoil()) continue;

            var soilNode = nearbyBlock.GetComponent<SoilNode>();
            if (soilNode) return soilNode.farmController;
        }

        return null;
    }
}