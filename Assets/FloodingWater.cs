using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FloodingWater : MonoBehaviour
{
    public GameObject fullHeightWaterBlockTemplate;
    public GameObject groundWaterBlockTemplate;
    private Block _block;
    private WorldPlane _worldPlane;

    void Start()
    {
        _block = GetComponentInChildren<Block>();
        _worldPlane = GameObject.FindWithTag("WorldPlane").GetComponent<WorldPlane>();
    }

    void Update()
    {
        if (_block.IsPermaFrozen()) return;

        if (_block.IsGroundLevel())
        {
            FloodGroundLevel();
        }
        else if (_worldPlane.IsBlockLowestWater(_block))
        {
            FloodDown();
        }

        _block.PermanentFreeze();
    }

    private void FloodDown()
    {
        var blockHeight = _block.GetGridPosition().y;
        var nearbyBlocks = _worldPlane
            .GetNearbyLots(_block.GetGridPosition())
            .Where(otherBlock => otherBlock.GetGridPosition().y < blockHeight);

        foreach (var nearbyBlock in nearbyBlocks)
        {
            if (nearbyBlock.OccupiedByHouse())
            {
                nearbyBlock.DestroyOccupant();
            }

            if (nearbyBlock.IsVacant())
            {
                Block lowestBlock = nearbyBlock;

                if (!nearbyBlock.IsGroundLevel())
                {
                    var waterBlockBelow = NewFullHeightWaterBlock().GetComponentInChildren<Block>();
                    _worldPlane.ReplaceBlock(nearbyBlock, waterBlockBelow);

                    lowestBlock = waterBlockBelow;
                }

                for (var y = lowestBlock.GetGridPosition().y + 1; y <= blockHeight; y++)
                {
                    var useFullHeightWater = y != blockHeight;
                    var water = useFullHeightWater ? NewFullHeightWaterBlock() : NewShallowWater();
                    var waterBlock = water.GetComponentInChildren<Block>();
                    _worldPlane.AddBlockOnTopOf(waterBlock, water, lowestBlock);

                    lowestBlock = waterBlock;
                }
            }
        }

        FloodGroundLevel();
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
        var gridPosition = GroundedGridPosition();

        var nearbyEmptyPositions = _worldPlane.GetNearbyEmptyPositions(gridPosition);
        foreach (var position in nearbyEmptyPositions)
        {
            var waterBlock = NewShallowWater().GetComponentInChildren<Block>();
            _worldPlane.AddAndPositionBlock(waterBlock, position);
            waterBlock.ShortFreeze();
        }
    }

    private Vector3 GroundedGridPosition()
    {
        var gridPosition = _block.GetGridPosition();
        gridPosition.y = 0;

        return gridPosition;
    }
}