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
        _block = GetComponent<Block>();
        _worldPlane = GameObject.FindWithTag("WorldPlane").GetComponent<WorldPlane>();
    }

    void Update()
    {
        if (_block.IsPermaFrozen()) return;

        if (_block.IsGroundLevel())
        {
            FloodGroundLevel();
        }
        else if (_block.IsLowestWater())
        {
            FloodDown();
        }
    }

    private void FloodDown()
    {
        var blockHeight = _block.GetPosition().y;
        var nearbyBlocks = _worldPlane
            .GetNearbyLots(_block.GetPosition())
            .Where(otherBlock => otherBlock.GetPosition().y < blockHeight);

        foreach (var nearbyBlock in nearbyBlocks)
        {
            if (nearbyBlock.OccupiedByHouse())
            {
                nearbyBlock.DestroyOccupant();
            }

            if (nearbyBlock.IsVacant())
            {
                var lowestBlock = nearbyBlock;
                
                if (!nearbyBlock.IsGroundLevel())
                {
                    var waterBlockBelow = newWaterBlock(true);
                    nearbyBlock.TurnOverSpotTo(waterBlockBelow);

                    lowestBlock = waterBlockBelow;
                }

                for (var y = lowestBlock.GetPosition().y + 1; y <= blockHeight; y++)
                {
                    var water = newWaterObject(y != blockHeight);
                    var waterBlock = water.GetComponentInChildren<Block>();
                    lowestBlock.PlaceOnTopOfSelf(waterBlock, water);

                    lowestBlock = waterBlock;
                }
            }
        }
    }

    private GameObject newWaterObject(bool useFullHeightWater)
    {
        if (useFullHeightWater)
        {
            return Instantiate(fullHeightWaterBlockTemplate);
        }
        else
        {
            return Instantiate(groundWaterBlockTemplate);
        }
    }

    private Block newWaterBlock(bool useFullHeightWater)
    {
        return newWaterObject(useFullHeightWater).GetComponentInChildren<Block>();
    }

    private void FloodGroundLevel()
    {
        var nearbyEmptyPositions = _worldPlane.GetNearbyEmptyPositions(_block.GetPosition());
        foreach (var position in nearbyEmptyPositions)
        {
            var waterBlock = newWaterBlock(false);
            _worldPlane.AddBlockToPosition(waterBlock, position);
            waterBlock.ShortFreeze();
        }
    }
}