using System;
using System.Collections;
using System.Collections.Generic;
using blockInteractions;
using UnityEngine;
using Random = System.Random;


[RequireComponent(typeof(BlockRelative))]
public class BuildingSpawn : MonoBehaviour
{
    public static int activeSpawns = 0;
    private static bool _placedFirstHouse = false;
    private WorldPlane _worldPlane;
    private WorkQueue _workQueue;
    private int _ticket;

    public AudioClip notPossibleSound;
    public BlockRelative blockRelative;

    public Vector3 spawnLookingTarget;
    public Block spawnLot;

    public Func<BuildingInfo> GetBuildingInfo = () => new BuildingInfo {devotees = 0};
    public Func<Vector3> GetTarget = () => Vector3.zero;
    public Func<GameObject> CreateBuildingAction = CreateTinyHouse;
    public Func<bool> CanStillConstruct = () => true;
    private bool _deactivated;
    private ConstructionMediator _constructionMediator;

    private void Awake()
    {
        blockRelative = GetComponent<BlockRelative>();
        activeSpawns += 1;
    }

    private void Start()
    {
        _worldPlane = WorldPlane.Get();
        _workQueue = WorkQueue.Get();
        _constructionMediator = ConstructionMediator.Get();
    }

    public static int ActiveSpawnCount()
    {
        return activeSpawns;
    }

    public void GroundHighlight(Block block)
    {
        var targetPosition = block.transform.position + Vector3.up * 0.1f;
        transform.position = targetPosition;
    }

    public void DestroyBuildingSpawn()
    {
        Deactivate();

        blockRelative.block.DestroyOccupant();
    }

    public void ActivateBuildingSpawn()
    {
        if (CanStillConstruct())
        {
            Deactivate();

            var block = blockRelative.block;
            block.DestroyOccupant();

            var target = GetTarget();
            if (target == Vector3.zero) target = spawnLookingTarget;
            PlaceHouse(spawnLot, target);

            _constructionMediator.BuildingCreated(GetBuildingInfo());
        }
        else
        {
            AudioSource.PlayClipAtPoint(notPossibleSound, spawnLot.transform.position, .02f * GameManager.MasterVolume);
        }
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
            activeSpawns -= 1;
        }
    }
}