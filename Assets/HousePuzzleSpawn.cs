using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using blockInteractions;
using UnityEngine;
using Random = System.Random;


public class HousePuzzleSpawn : MonoBehaviour
{
    private static bool _placedFirstHouse = false;
    private WorldPlane _worldPlane;
    private WorkQueue _workQueue;
    private int _ticket;

    public AudioClip notPossibleSound;

    public Vector3 spawnLookingTarget;
    public Vector3 spawnGridPosition;

    private bool _deactivated;
    private ConstructionMediator _constructionMediator;
    private Animator _animator;
    private static readonly int Highlight = Animator.StringToHash("Highlight");

    private void Awake()
    {
        BuildingSpawn.activeSpawns += 1;

        _animator = GetComponentInChildren<Animator>();
    }

    private void Start()
    {
        _worldPlane = WorldPlane.Get();
        _workQueue = WorkQueue.Get();
        _constructionMediator = ConstructionMediator.Get();
        _animator.SetTrigger(Highlight);
    }

    public BuildingInfo GetBuildingInfo()
    {
        return new BuildingInfo {devotees = 10};
    }
    
    public Vector3 GetTarget()
    {
        return spawnLookingTarget;
    }
    
    public GameObject CreateBuildingAction()
    {
        return BlockFactory.Get().TinyHouse();
    }
    
    public bool CanStillConstruct()
    {
        var spawnLot = GetSpawnBlock();
        return _worldPlane
            .GetNearbyBlocks(spawnLot.GetGridPosition())
            .Count(block => block.IsWater() &&  block.GetHeight() == spawnLot.GetHeight()) > 0;
    }

    public void DestroyBuildingSpawn()
    {
        Deactivate();

        Destroy(gameObject);
    }

    public void ActivateBuildingSpawn()
    {
        if (CanStillConstruct())
        {
            var target = GetTarget();
            if (target == Vector3.zero) target = spawnLookingTarget;
            PlaceHouse(target);

            _constructionMediator.BuildingCreated(GetBuildingInfo());

            DestroyBuildingSpawn();
        }
        else
        {
            AudioSource.PlayClipAtPoint(notPossibleSound, _worldPlane.ToRealCoordinates(spawnGridPosition), .02f * GameManager.MasterVolume);
        }
    }

    private void PlaceHouse(Vector3 lookingTarget)
    {
        var lot = GetSpawnBlock();
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

    private void Deactivate()
    {
        if (!_deactivated)
        {
            _deactivated = true;
            BuildingSpawn.activeSpawns -= 1;
        }
    }
    void Update()
    {
        if (!CanWorkThisFrame()) return;

        var topBlock = _worldPlane.GetBlockAtTopOfStack(spawnGridPosition);
        transform.position = topBlock.transform.position + Vector3.up * .1f;
    }
    
    private bool CanWorkThisFrame()
    {
        if (_workQueue.HasExpiredTicket(_ticket))
        {
            _ticket = _workQueue.GetTicket();
        }

        return _workQueue.HasTicketForFrame(_ticket);
    }

    private Block GetSpawnBlock()
    {
        return _worldPlane.GetBlockAtTopOfStack(spawnGridPosition);
    }

}