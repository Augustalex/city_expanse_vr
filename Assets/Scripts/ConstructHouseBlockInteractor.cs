using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class ConstructHouseBlockInteractor : BlockInteractor
{
    public GameObject tinyHouseTemplate;

    public override bool Interactable(GameObject other)
    {
        if (!IsActivated()) return false;

        var blockComponent = other.gameObject.GetComponent<Block>();

        return blockComponent &&
               blockComponent.IsLot() &&
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
            .Where(otherBlock => otherBlock.IsWater() && otherBlock.IsOnSameHeightAs(vacantLot))
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
        else
        {
            SpawnInnerCityHouse(other);
        }
    }

    private void SpawnInnerCityHouse(GameObject other)
    {
        var vacantLot = other.gameObject.GetComponent<Block>();

        var candidates = GetWorldPlane().GetNearbyBlocks(vacantLot.GetGridPosition())
            .Where(otherBlock => otherBlock.OccupiedByHouse() && otherBlock.GetOccupantHouse().IsBig())
            .ToList();

        if (candidates.Count > 0)
        {
            var bigHouse = candidates.ElementAt(Random.Range(0, candidates.Count));
            var house = Instantiate(tinyHouseTemplate);
            house.GetComponent<HouseSpawn>().SetIsInnerCity();
            vacantLot.Occupy(house);

            var target = bigHouse.transform.position;
            target.y = house.transform.position.y;
            house.transform.LookAt(target);
        }
    }
}