using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class FloodingWater : MonoBehaviour
{
    public GameObject fullHeightWaterBlockTemplate;
    public GameObject groundWaterBlockTemplate;
    public bool isOceanWater;

    private Block _block;
    private WorldPlane _worldPlane;
    private float _life;

    private int _ticket = -1;
    private WorkQueue _workQueue;
    private bool _flood;
    private BlockFactory _blockFactory;

    void Start()
    {
        _life = Time.fixedTime;
        _block = GetComponentInChildren<Block>();
        _worldPlane = GameObject.FindWithTag("WorldPlane").GetComponent<WorldPlane>();
        _workQueue = WorkQueue.Get();
        _blockFactory = BlockFactory.Get();

        _block.SetAsUnstable();
    }

    void Update()
    {
        if (!_worldPlane.WorldGenerationDone()) return;
        
        if (!_block.IsWaterLocked() && Random.value < .01f)
        {
            UpdateWaterLockedStatus();
        }

        if (_block.IsPermaFrozen()) return;

        if (Time.fixedTime - _life > 1.25f)
        {
            _flood = true;
        }

        if (_flood)
        {
            if (CanFloodThisFrame())
            {
                if (isOceanWater)
                {
                    FloodAllAsOcean();
                }
                else
                {
                    FloodAll();
                }

                SetAsStable();
            }
        }
    }

    private void UpdateWaterLockedStatus()
    {
        _block.SetWaterLocked(_worldPlane.GetNearbyBlocks(_block.GetGridPosition())
            .All(block => block.IsWater()));
    }

    private bool CanFloodThisFrame()
    {
        if (_workQueue.HasExpiredTicket(_ticket))
        {
            _ticket = _workQueue.GetTicket();
        }

        return _workQueue.HasTicketForFrame(_ticket);
    }

    private void SetAsStable()
    {
        _block.SetAsStable();
        _block.PermanentFreeze();
    }

    private void FloodAll()
    {
        _flood = false;
        var blockHeight = _block.GetGridPosition().y;
        var nearbyBlocks = _worldPlane
            .GetNearbyLand(_block.GetGridPosition())
            .Where(otherBlock => otherBlock.GetGridPosition().y < blockHeight);
        var nearbyEmptyBlocks = CreateWaterForNearbyEmptyPositions();
        var allNearbyBlocks = nearbyBlocks.Concat(nearbyEmptyBlocks);
        foreach (var nearbyBlock in allNearbyBlocks)
        {
            if (nearbyBlock.HasOccupant() && !nearbyBlock.OccupiedByBlock())
            {
                nearbyBlock.DestroyOccupant();
            }

            if (nearbyBlock.IsVacant())
            {
                Block bottomBlock = nearbyBlock;
                Block lowestBlock = nearbyBlock;

                var nearbyBlockIsAtLowestLevel = nearbyBlock.GetHeight() == (int) Block.LowestLevel;
                if (!nearbyBlock.IsWater() && !nearbyBlockIsAtLowestLevel)
                {
                    var waterBlockBelow = NewFullHeightWaterBlock().GetComponentInChildren<Block>();
                    _worldPlane.ReplaceBlock(nearbyBlock, waterBlockBelow);

                    bottomBlock = waterBlockBelow;
                    lowestBlock = waterBlockBelow;
                }

                for (var y = lowestBlock.GetGridPosition().y + 1; y < blockHeight || y <= bottomBlock.GetHeight() + 1; y++)
                {
                    var useFullHeightWater = Math.Abs(y - blockHeight) > .5f;
                    var water = useFullHeightWater ? NewFullHeightWaterBlock() : NewShallowWater();
                    var waterBlock = water.GetComponentInChildren<Block>();
                    _worldPlane.AddBlockOnTopOf(waterBlock, water, lowestBlock);

                    lowestBlock = waterBlock;
                }
            }
        }
    }

    private void FloodAllAsOcean()
    {
        _flood = false;
        var blockHeight = _block.GetGridPosition().y;
        var nearbyBlocks = _worldPlane
            .GetNearbyLandOrLake(_block.GetGridPosition())
            .Where(otherBlock => otherBlock.GetGridPosition().y < blockHeight);
        var nearbyEmptyBlocks = CreateWaterForNearbyEmptyPositions();
        var allNearbyBlocks = nearbyBlocks.Concat(nearbyEmptyBlocks);

        foreach (var nearbyBlock in allNearbyBlocks)
        {
            if (nearbyBlock.HasOccupant() && !nearbyBlock.OccupiedByBlock())
            {
                nearbyBlock.DestroyOccupant();
            }

            if (nearbyBlock.IsVacant())
            {
                var waterBlockBelow = NewOceanFullHeightWaterBlock().GetComponentInChildren<Block>();
                _worldPlane.ReplaceBlock(nearbyBlock, waterBlockBelow);

                var lowestBlock = waterBlockBelow;

                for (var y = lowestBlock.GetGridPosition().y + 1; y <= Block.GroundLevel - 1; y++)
                {
                    var water = NewOceanFullHeightWaterBlock();
                    var waterBlock = water.GetComponentInChildren<Block>();
                    _worldPlane.AddBlockOnTopOf(waterBlock, water, lowestBlock);

                    lowestBlock = waterBlock;
                }

                var ocean = NewOceanShallowWaterBlock();
                var oceanBlock = ocean.GetComponentInChildren<Block>();
                _worldPlane.AddBlockOnTopOf(oceanBlock, ocean, lowestBlock);
            }
        }
    }

    private GameObject NewOceanShallowWaterBlock()
    {
        return Instantiate(_blockFactory.oceanShallowBlockTemplate);
    }

    private GameObject NewOceanFullHeightWaterBlock()
    {
        return Instantiate(_blockFactory.oceanFullHeightBlockTemplate);
    }

    private GameObject NewFullHeightWaterBlock()
    {
        return Instantiate(fullHeightWaterBlockTemplate);
    }

    private GameObject NewShallowWater()
    {
        return Instantiate(groundWaterBlockTemplate);
    }

    private void FloodGroundLevel()
    {
        var gridPosition = LowestGridPosition();

        var nearbyEmptyPositions = _worldPlane.GetNearbyEmptyPositionsStream(gridPosition);
        foreach (var position in nearbyEmptyPositions)
        {
            var waterBlock = NewShallowWater().GetComponentInChildren<Block>();
            _worldPlane.AddAndPositionBlock(waterBlock, position);
            waterBlock.ShortFreeze();
        }
    }

    private IEnumerable<Block> CreateWaterForNearbyEmptyPositions()
    {
        var gridPosition = LowestGridPosition();

        return _worldPlane.GetNearbyEmptyPositionsStream(gridPosition)
            .Select(position =>
            {
                var waterBlock = NewFullHeightWaterBlock().GetComponentInChildren<Block>();
                _worldPlane.AddAndPositionBlock(waterBlock, position);
                waterBlock.ShortFreeze();
                return waterBlock;
            });
    }

    private Vector3 LowestGridPosition()
    {
        var gridPosition = _block.GetGridPosition();
        gridPosition.y = Block.LowestLevel;

        return gridPosition;
    }

    public void Resurrect()
    {
        _block.UnPermaFreeze();
        _block.SetAsUnstable();
        _life = Time.fixedTime;
    }
}