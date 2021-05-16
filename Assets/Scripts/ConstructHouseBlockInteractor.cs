using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;
using Random = UnityEngine.Random;

public class ConstructHouseBlockInteractor : BlockInteractor
{
    public GameObject tinyHouseTemplate;
    public GameObject ghostTemplate;
    public GameObject nonInteractableGhostTemplate;
    private GameObject _ghost;
    private GameObject _nonInteractableGhost;
    private FeatureToggles _featureToggles;
    private bool _started;

    private new void Start()
    {
        base.Start();

        _featureToggles = FeatureToggles.Get();

        _ghost = Instantiate(ghostTemplate);
        _ghost.SetActive(false);

        _nonInteractableGhost = Instantiate(nonInteractableGhostTemplate);
        _nonInteractableGhost.SetActive(false);

        _started = true;
    }

    public override void Deactivate()
    {
        if (_started && _featureToggles.houseGhosts)
        {
            _nonInteractableGhost.SetActive(false);
            _ghost.SetActive(false);
        }

        base.Deactivate();
    }

    public override void Inspect(GameObject other)
    {
        if (!_started) return;

        if (_featureToggles.houseGhosts)
        {
            var blockComponent = other.gameObject.GetComponent<Block>();
            if (!blockComponent) return;

            if (Interactable(other))
            {
                if (_nonInteractableGhost.activeSelf) _nonInteractableGhost.SetActive(false);
                if (!_ghost.activeSelf) _ghost.SetActive(true);
                _ghost.transform.position = other.transform.position + Vector3.up * .15f;
            }
            else
            {
                if (_ghost.activeSelf) _ghost.SetActive(false);
                if (!_nonInteractableGhost.activeSelf) _nonInteractableGhost.SetActive(true);
                _nonInteractableGhost.transform.position = other.transform.position + Vector3.up * .15f;
            }
        }
        else
        {
            base.Inspect(other);
        }
    }


    public override bool Interactable(GameObject other)
    {
        if (!IsActivated()) return false;

        var blockComponent = other.gameObject.GetComponent<Block>();

        return blockComponent &&
               blockComponent.IsLot() &&
               blockComponent.IsVacant() &&
               blockComponent.IsInteractable() &&
               GetCandidate(other) != null;
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

    [CanBeNull]
    private Block GetCandidate(GameObject other)
    {
        var vacantLot = other.gameObject.GetComponent<Block>();

        var candidates = GetWorldPlane().GetNearbyBlocks(vacantLot.GetGridPosition())
            .Where(otherBlock => otherBlock.IsWater() && otherBlock.IsOnSameHeightAs(vacantLot))
            .ToList();

        var candidatesCount = candidates.Count;
        if (candidatesCount > 0)
        {
            return candidates[Random.Range(0, candidatesCount)];
        }

        var innerCityCandidates = GetWorldPlane().GetNearbyBlocks(vacantLot.GetGridPosition())
            .Where(otherBlock => otherBlock.OccupiedByHouse() && otherBlock.GetOccupantHouse().IsBig())
            .ToList();

        if (innerCityCandidates.Count > 0)
        {
            return innerCityCandidates.ElementAt(Random.Range(0, innerCityCandidates.Count));
        }

        return null;
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