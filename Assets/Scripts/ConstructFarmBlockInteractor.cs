using System;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class ConstructFarmBlockInteractor : BlockInteractor
{
    public override InteractorHolder.BlockInteractors InteractorType => InteractorHolder.BlockInteractors.ConstructFarm;

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
               (IsSoilCandidate(other)
                || IsMasterFarmCandidate(other));
    }

    public override bool LockOnLayer()
    {
        return true;
    }

    public override void Interact(GameObject other)
    {
        if (IsSoilCandidate(other))
        {
            SoilInteraction(other);
        }
        else
        {
            MasterFarmInteraction(other);
        }
    }

    private void SoilInteraction(GameObject other)
    {
        var vacantLot = other.gameObject.GetComponent<Block>();

        var nearbyFarmController = GetClosestFarmController(vacantLot.GetGridPosition());
        nearbyFarmController.AddBlockToSoilNetwork(vacantLot);
    }

    private bool IsMasterFarmCandidate(GameObject other)
    {
        var vacantLot = other.gameObject.GetComponent<Block>();

        var nextToInnerCityHouse = GetWorldPlane()
            .GetNearbyBlocks(vacantLot.GetGridPosition())
            .Any(b => b.OccupiedByHouse());

        return nextToInnerCityHouse;
    }

    private void MasterFarmInteraction(GameObject other)
    {
        var vacantLot = other.gameObject.GetComponent<Block>();
        FarmMasterController.Get().SetupFarmControllerForBlock(vacantLot);
    }

    private bool IsSoilCandidate(GameObject other)
    {
        var vacantLot = other.gameObject.GetComponent<Block>();
        
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