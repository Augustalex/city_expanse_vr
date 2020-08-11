﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WorldPlane : MonoBehaviour
{
    public BlocksRepository blocksRepository = new BlocksRepository();
    public GameObject blockTemplate;
    public Transform TopLeftPoint;

    public enum Size
    {
        Small,
        Medium,
        Large
    }

    private Vector2 _currentDimensions;
    private static WorldPlane _worldPlaneInstance;

    private void Awake()
    {
        _worldPlaneInstance = this;
    }

    public static WorldPlane Get()
    {
        return _worldPlaneInstance;
    }

    public Vector3 ToRealCoordinates(Vector3 position)
    {
        var placementScale = blockTemplate.GetComponent<PlacementScale>();
        var yScale = placementScale ? placementScale.yScale : blockTemplate.transform.localScale.y;

        var x = position.x;
        var z = position.z;
        var middle = Math.Abs(x % 2) < .5f;
        var xOffset = (((blockTemplate.transform.localScale.x + -0.014f) * x)) +
                      blockTemplate.transform.localScale.x * .5f;
        var zOffset = blockTemplate.transform.localScale.z * z + blockTemplate.transform.localScale.z * .5f +
                      (middle ? blockTemplate.transform.localScale.z * -.5f : 0);
        return TopLeftPoint.position + new Vector3(
            xOffset,
            position.y * yScale + yScale * .5f,
            zOffset
        );
    }

    public void RemoveAndDestroyBlock(Block block)
    {
        block.DestroySelf();
        RemoveBlockAt(block.GetGridPosition());
    }

    public void RemoveBlockAt(Vector3 position)
    {
        blocksRepository.Remove(position);
    }

    private IEnumerable<Vector3> GetNeighbouringPositions(Vector3 position)
    {
        var middle = Math.Abs(position.x % 2) < .5f;
        var positionY = 0;
        var translations = new List<Vector3>
        {
            new Vector3(-1, positionY, 0),
            new Vector3(-1, positionY, middle ? -1 : 1),

            new Vector3(0, positionY, -1),
            new Vector3(0, positionY, 1),

            new Vector3(1, positionY, 0),
            new Vector3(1, positionY, middle ? -1 : 1),
        };
        var nearbyPositions = translations
            .Select(translation =>
            {
                var translatedPosition = position + translation;
                translatedPosition.y = GetStackHeight(translatedPosition);

                return translatedPosition;
            })
            .Where(IsWithinBounds);

        return nearbyPositions;
    }

    public List<Vector3> GetNearbyEmptyPositions(Vector3 position)
    {
        return GetNeighbouringPositions(position)
            .Where(newPosition => !blocksRepository.HasAtPosition(newPosition))
            .ToList();
    }

    public bool IsBlockLowestWater(Block block)
    {
        if (!block.IsWater()) return false;

        var position = block.GetGridPosition();

        var lowestWater = GetStack(position).First(otherBlock => otherBlock.IsWater());
        return Math.Abs(lowestWater.GetGridPosition().y - position.y) < .5f;
    }

    public void AddBlockOnTopOf(Block blockToAdd, GameObject blockToAddRoot, Block blockAtBottom)
    {
        var gridPosition = blockAtBottom.GetGridPosition() + Vector3.up;
        blockToAdd.SetGridPosition(gridPosition);
        blocksRepository.SetAtPosition(blockToAdd, gridPosition);
        MakeBlockMoreUnique(blockToAdd, gridPosition);

        blockAtBottom.PlaceOnTopOfSelf(blockToAdd, blockToAddRoot);
    }

    public void AddAndPositionBlock(Block block, Vector3 gridPosition)
    {
        MakeBlockMoreUnique(block, gridPosition);
        blocksRepository.SetAtPosition(block, gridPosition);

        PositionBlock(block, gridPosition);
    }

    private void PositionBlock(Block block, Vector3 gridPosition)
    {
        block.SetGridPosition(gridPosition);
        block.transform.position =
            ToRealCoordinates(
                gridPosition); // TODO Should fix so that the block.BlockRoot() is the one being moved, but right now there is a weird bug when doing that...
    }

    private void MakeBlockMoreUnique(Block block, Vector3 position)
    {
        block.RandomRotateAlongY();
        MakeSureTopGrassBlocksHaveCorrectTexture(position);
    }

    public void ReplaceBlock(Block toBeReplaced, Block replacement)
    {
        var gridPosition = toBeReplaced.GetGridPosition();

        RemoveBlockAt(gridPosition);
        AddAndPositionBlock(replacement, gridPosition);

        toBeReplaced.DestroySelf();
    }

    private void MakeSureTopGrassBlocksHaveCorrectTexture(Vector3 position)
    {
        var stack = blocksRepository.StreamPairs().Where(pair => pair.Key.x == position.x && pair.Key.z == position.z);

        var list = stack.OrderByDescending(pair => pair.Key.y).ToList();
        for (var i = 0; i < list.Count; i++)
        {
            var element = list[i];
            var isTopBlock = i == 0;

            if (!element.Value.IsGrass()) continue;

            if (isTopBlock)
            {
                element.Value.GetComponent<GrassBlock>().SetTopMaterial();
            }
            else
            {
                element.Value.GetComponent<GrassBlock>().SetNormalMaterial();
            }
        }
    }

    public List<Block> GetWaterBlocks()
    {
        return blocksRepository.StreamBlocks()
            .Where(b => b.blockType == Block.BlockType.Water)
            .ToList();
    }

    public IEnumerable<Block> GetNearbyBlocksWithinRange(Vector3 position, float radius)
    {
        return blocksRepository.StreamPairs()
            .Where(pair => Vector3.Distance(position, pair.Key) < radius)
            .Select(pair => pair.Value);
    }

    public IEnumerable<Block> GetNearbyBlocks(Vector3 position)
    {
        return GetNeighbouringPositions(position)
            .Where(newPosition => blocksRepository.HasAtPosition(newPosition))
            .Select(newPosition => blocksRepository.GetAtPosition(newPosition));
    }

    public List<Block> GetNearbyLots(Vector3 position)
    {
        return GetNearbyBlocks(position)
            .Where(b => b.blockType == Block.BlockType.Grass)
            .ToList();
    }

    public List<Block> GetNearbyVacantLots(Vector3 position)
    {
        return GetNearbyLots(position)
            .Where(block => block.IsVacant())
            .ToList();
    }

    public List<Block> GetVacantBlocks()
    {
        return blocksRepository.StreamPairs()
            .Where(pair => pair.Value.IsVacant())
            .Select(pair => pair.Value)
            .ToList();
    }

    public List<Block> GetBlocksWithHouses()
    {
        return blocksRepository.StreamBlocks().Where(block => block.OccupiedByHouse()).ToList();
    }

    public int GetStackHeight(Vector3 position)
    {
        var stack = blocksRepository.StreamPairs().Where(pair => pair.Key.x == position.x && pair.Key.z == position.z);

        var list = stack.ToList();
        if (list.Count == 0) return 0;

        return Convert.ToInt32(list.Max(pair => pair.Key.y));
    }

    public List<Block> GetStack(Vector3 position)
    {
        var stack = blocksRepository.StreamPairs().Where(pair => pair.Key.x == position.x && pair.Key.z == position.z);

        return stack.Select(pair => pair.Value).ToList();
    }

    public float NatureScore(Vector3 originPosition, float radius)
    {
        var realCenterPosition = ToRealCoordinates(originPosition);
        var sizeOfBlock = blockTemplate.transform.localScale.x;

        var hits = Physics.OverlapSphere(realCenterPosition, sizeOfBlock * radius * .75f);

        return hits
            .Select(hit => hit.gameObject.GetComponent<Block>())
            .Where(block => block != null)
            .Sum(block =>
            {
                if (block.OccupiedByHouse() && block.GetOccupantHouse().IsMegaBig()) return -20;
                if (block.OccupiedByHouse() && block.GetOccupantHouse().IsBig()) return -10;
                if (block.OccupiedByHouse() && block.GetOccupantHouse()) return -1;

                if (block.OccupiedByGreens() && block.GetOccupantGreens().IsGrown() && block.GetGridPosition().y > 10) return 20;
                if (block.OccupiedByGreens() && block.GetOccupantGreens().IsGrown() && block.GetGridPosition().y > 4) return 10;
                if (block.OccupiedByGreens() && block.GetOccupantGreens().IsGrown() && block.GetGridPosition().y > 2) return 6;
                if (block.OccupiedByGreens() && block.GetOccupantGreens().IsGrown() && block.GetGridPosition().y > 0) return 4;
                if (block.OccupiedByGreens() &&  block.GetOccupantGreens().IsGrown()) return 2;

                if (block.IsGrass() && block.GetGridPosition().y > 10) return 4;
                if (block.IsGrass() && block.GetGridPosition().y > 4) return 2;
                if (block.IsGrass() && block.GetGridPosition().y > 2) return 1;
                if (block.IsGrass() && block.GetGridPosition().y > 0) return .5f;

                if (block.IsWater() && block.GetGridPosition().y > 10) return 4;
                if (block.IsWater() && block.GetGridPosition().y > 4) return 4;
                if (block.IsWater() && block.GetGridPosition().y > 2) return 2;

                if (block.IsSand()) return -1;

                return 0;
            });

        // return blocksRepository.StreamPairs()
        //     .Where(pair => Vector3.Distance(originPosition, pair.Key) < radius)
        //     .Select(pair => pair.Value)
        //     .Sum(block =>
        //     {
        //         if (block.OccupiedByHouse() && block.GetOccupantHouse().IsMegaBig()) return -20;
        //         if (block.OccupiedByHouse() && block.GetOccupantHouse().IsBig()) return -10;
        //         if (block.OccupiedByHouse() && block.GetOccupantHouse()) return -1;
        //
        //         if (block.OccupiedByGreens() && block.GetGridPosition().y > 10) return 20;
        //         if (block.OccupiedByGreens() && block.GetGridPosition().y > 4) return 10;
        //         if (block.OccupiedByGreens() && block.GetGridPosition().y > 2) return 6;
        //         if (block.OccupiedByGreens() && block.GetGridPosition().y > 0) return 4;
        //         if (block.OccupiedByGreens()) return 2;
        //
        //         if (block.IsGrass() && block.GetGridPosition().y > 10) return 4;
        //         if (block.IsGrass() && block.GetGridPosition().y > 4) return 2;
        //         if (block.IsGrass() && block.GetGridPosition().y > 2) return 1;
        //         if (block.IsGrass() && block.GetGridPosition().y > 0) return .5f;
        //
        //         if (block.IsWater() && block.GetGridPosition().y > 10) return 4;
        //         if (block.IsWater() && block.GetGridPosition().y > 4) return 4;
        //         if (block.IsWater() && block.GetGridPosition().y > 2) return 2;
        //         if (block.IsWater()) return 1;
        //
        //         if (block.IsSand()) return -1;
        //
        //         return 0;
        //     });
    }

    public void ResetAtSize(Size size)
    {
        var allBlocks = blocksRepository.StreamBlocks().ToList();
        foreach (var block in allBlocks)
        {
            try
            {
                RemoveAndDestroyBlock(block);
            }
            catch
            {
                Debug.Log("Not all blocks were successfully destroyed");
            }
        }

        blocksRepository.RemoveAll();
        blocksRepository = new BlocksRepository();

        // TODO Fix issue where houses won't spawn when got farms and when has Destroyed world with meteor at least once
        // TODO Make it possible to spawn multiple big houses again
        // TODO Make big houses have a LARGE negative impact on Nature score
        // TODO Make world a lot bigger -> Make more clouds? Create clouds from soaking up water?
        // TODO Navigation using hand controls.. how?
        // TODO Debug performance issues -> Try replacing houses with low poly meshes! Try run some profiler? Maybe some cached matrix of all blocks and all it's different lookup variants?

        StartCoroutine(CreateWorldAsync());

        IEnumerator CreateWorldAsync()
        {
            yield return CreateWorld(SizeToDimensions(size));
        }
    }


    private Vector2 SizeToDimensions(Size size)
    {
        switch (size)
        {
            case Size.Small:
                return new Vector2(3, 3);
            case Size.Medium:
                return new Vector2(9, 9);
            case Size.Large:
                return new Vector2(24, 24);
            default:
                throw new Exception("There is no dimensions specified for size: " + size);
        }
    }

    private IEnumerator CreateWorld(Vector2 dimensions)
    {
        _currentDimensions = dimensions;

        var yChunks = 8;
        for (var yChunk = 1; yChunk <= yChunks; yChunk++)
        {
            var xChunks = 8;
            for (var xChunk = 1; xChunk <= xChunks; xChunk++)
            {
                var rowHeight = Mathf.Ceil(dimensions.y / yChunks);
                var yStart = rowHeight * (yChunk - 1);
                var yMax = rowHeight * yChunk;
                for (var row = yStart; row < yMax; row++)
                {
                    var columnHeight = Mathf.Ceil(dimensions.x / xChunks);
                    var xStart = columnHeight * (xChunk - 1);
                    var xMax = columnHeight * xChunk;
                    for (var column = Math.Abs(xStart) < .5f ? -1 : xStart; column < xMax; column++)
                    {
                        var isMiddle = Math.Abs(row % 2) < .5f;
                        if (isMiddle && Math.Abs(column - (-1)) < .5f) continue;

                        var blockPosition = new Vector3(row, 0, column);
                        var blockObject = Instantiate(blockTemplate);
                        var block = blockObject.GetComponentInChildren<Block>();

                        AddAndPositionBlock(block, blockPosition);
                    }
                    
                    yield return new WaitForEndOfFrame();
                }
            }
        }
    }

    private bool IsWithinBounds(Vector3 point)
    {
        var row = point.x;
        var column = point.z;
        var isEven = Math.Abs(row % 2) < .5f;

        if (isEven)
        {
            return row >= 0 && row < _currentDimensions.x
                            && column >= 0 && column < _currentDimensions.y;
        }
        else
        {
            return row >= 0 && row < _currentDimensions.x
                            && column >= -1 && column < _currentDimensions.y;
        }
    }

    public int CountBlocksOfType(Block.BlockType blockType)
    {
        return blocksRepository.StreamBlocks().Count(block => block.blockType == blockType);
    }

    public IEnumerable<Block> GetBlocksWithHousesNotNearWater()
    {
        return GetBlocksWithHouses()
            .Where(block =>
            {
                var amountOfNeighbouringWaterBlocks = GetNearbyBlocks(block.GetGridPosition())
                    .Count(otherBlock => otherBlock.IsWater());

                return amountOfNeighbouringWaterBlocks == 0;
            });
    }

    public Vector3 GetRealCenterPoint()
    {
        var centerGridPosition = new Vector3(Mathf.Round(_currentDimensions.x * .5f), 0,
            Mathf.Round(_currentDimensions.y * .5f));
        return ToRealCoordinates(centerGridPosition);
    }

    public float Top()
    {
        return TopLeftPoint.position.z;
    }

    public float Right()
    {
        return TopLeftPoint.position.x;
    }

    public float Bottom()
    {
        return TopLeftPoint.position.z + blockTemplate.transform.localScale.z * _currentDimensions.y;
    }

    public float Left()
    {
        return TopLeftPoint.position.x +
               blockTemplate.transform.localScale.x * _currentDimensions.x; //Should it be .y and not .x ...
    }

    public IEnumerable<Block> GetBlocksWithGreens()
    {
        return blocksRepository
            .StreamBlocks()
            .Where(block => block.OccupiedByGreens() && block.GetOccupantGreens().IsGrown());
    }

    public IEnumerable<Block> GetBlocksWithWoodcutters()
    {
        return blocksRepository
            .StreamBlocks()
            .Where(block => block.HasOccupant() && block.GetOccupant().GetComponent<WoodcutterSpawn>());
    }
}