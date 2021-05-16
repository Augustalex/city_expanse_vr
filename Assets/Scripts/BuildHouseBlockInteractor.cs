using System;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class BuildHouseBlockInteractor : BlockInteractor
{
    public override InteractorHolder.BlockInteractors InteractorType => InteractorHolder.BlockInteractors.ConstructHouse;

    public GameObject tinyHouseTemplate;

    public override bool Interactable(GameObject other)
    {
        if (!IsActivated()) return false;

        var blockComponent = other.gameObject.GetComponent<Block>();

        return blockComponent &&
               blockComponent.blockType == Block.BlockType.Grass &&
               blockComponent.IsVacant() &&
               blockComponent.IsInteractable();
    }

    public override bool LockOnLayer()
    {
        return true;
    }

    public override void Interact(GameObject other)
    {
        var vacantLot = other.gameObject.GetComponent<Block>();
        
        var candidates = GetWorldPlane().GetNearbyBlocks(vacantLot.GetGridPosition())
            .Where(otherBlock => otherBlock.IsWater() && otherBlock.IsLevelWith(vacantLot))
            .ToList();
        
        var candidatesCount = candidates.Count;
        if (candidatesCount > 0)
        {
            var waterBlock = candidates[Random.Range(0, candidatesCount)];
            var house = Instantiate(tinyHouseTemplate);
            vacantLot.Occupy(house);
            var target = waterBlock.transform.position;
            target.y = house.transform.position.y;
            house.transform.LookAt(target);
            
            PlaySound(BlockSoundLibrary.BlockSound.RaiseLand);
        }
    }
}