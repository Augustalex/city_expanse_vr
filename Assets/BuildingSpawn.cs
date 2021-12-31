﻿using System;
using System.Collections;
using System.Collections.Generic;
using blockInteractions;
using UnityEngine;


[RequireComponent(typeof(BlockRelative))]
public class BuildingSpawn : MonoBehaviour
{
    private static int _activeSpawns = 0;
    private static bool _placedFirstHouse = false;

    public BlockRelative blockRelative;

    public Vector3 spawnLookingTarget;
    public Block spawnLot;

    public Func<GameObject> CreateBuildingAction = CreateTinyHouse;
    private bool _deactivated;

    private void Awake()
    {
        blockRelative = GetComponent<BlockRelative>();
        _activeSpawns += 1;
    }

    public static int ActiveSpawnCount()
    {
        return _activeSpawns;
    }

    public void GroundHighlight(Block block)
    {
        var targetPosition = block.transform.position + Vector3.up * 0.1f;
        transform.position = targetPosition;
    }

    public void DestroyLakeSpawn()
    {
        Deactivate();
        
        blockRelative.block.DestroyOccupant();
    }

    public void ActivateBuildingSpawn()
    {
        Deactivate();
        
        var block = blockRelative.block;
        block.DestroyOccupant();

        PlaceHouse(spawnLot, spawnLookingTarget);

        var sound = BlockSoundLibrary.Get().GetSound(BlockSoundLibrary.BlockSound.Water);
        AudioSource.PlayClipAtPoint(sound, block.transform.position, .02f * GameManager.MasterVolume);
    }

    private void PlaceHouse(Block lot, Vector3 lookingTarget)
    {
        var house = CreateBuildingAction();
        lot.Occupy(house);
        var target = lookingTarget;
        target.y = house.transform.position.y;
        house.transform.LookAt(target);

        if (!_placedFirstHouse)
        {
            _placedFirstHouse = true;
            DiscoveryManager.Get().RegisterNewDiscover(DiscoveryManager.Discoverable.House);
        }
    }

    private static GameObject CreateTinyHouse()
    {
        return BlockFactory.Get().TinyHouse();
    }

    private void Deactivate()
    {
        if (!_deactivated)
        {
            _deactivated = true;
            _activeSpawns -= 1;
        }
    }
}