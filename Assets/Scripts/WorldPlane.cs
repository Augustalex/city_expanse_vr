﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class WorldPlane : MonoBehaviour
{
    public BlocksRepository blocksRepository = new BlocksRepository();
    public GameObject blockTemplate;
    public GameObject TopLeftPoint;
    public bool loadExistingWorld;
    public int waterLimit = 12; // NOTE: Change this to experiment how much sand takes over water in the desert

    private Transform _topLeftPointTransform;

    public enum Size
    {
        Small,
        Medium,
        Large
    }

    private Vector2 _currentDimensions;
    private Vector2 _currentMinBound = new Vector2(0, 0);
    private static WorldPlane _worldPlaneInstance;
    private bool _doneGeneratingWorld = false;
    private bool _worldIsEnding;

    private void Awake()
    {
        _worldPlaneInstance = this;
        _topLeftPointTransform = TopLeftPoint.transform;
    }

    private void Start()
    {
        // NOTE: Not working correctly yet
        if (loadExistingWorld)
        {
            var blocks = new List<Tuple<Block, Vector3>>();
            foreach (Transform child in _topLeftPointTransform)
            {
                var blockObject = child.gameObject;
                var block = blockObject.GetComponentInChildren<Block>();

                var blockPosition = ToGridPosition(blockObject.transform.position);

                blocks.Add(new Tuple<Block, Vector3>(block, blockPosition));
            }

            _currentMinBound = new Vector2(
                blocks.Min(tuple => tuple.Item2.x),
                blocks.Min(tuple => tuple.Item2.y)
            );

            foreach (var (block, position) in blocks)
            {
                AddAndPositionBlock(block, position);
            }
        }
    }

    public static WorldPlane Get()
    {
        return _worldPlaneInstance;
    }

    public bool WorldGenerationDone()
    {
        return _doneGeneratingWorld;
    }

    public bool WorldIsEnding()
    {
        return _worldIsEnding;
    }

    public void SetWorldIsEnding()
    {
        _worldIsEnding = true;
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
        return _topLeftPointTransform.position + new Vector3(
            xOffset,
            position.y * yScale + yScale * .5f,
            zOffset
        );
    }

    public Vector3 ToGridPosition(Vector3 position)
    {
        var zeroBasedPosition = position - _topLeftPointTransform.position;
        zeroBasedPosition.x /= blockTemplate.transform.localScale.x;
        zeroBasedPosition.z /= blockTemplate.transform.localScale.z;
        zeroBasedPosition.y -= .1f;

        return new Vector3(Mathf.Round(zeroBasedPosition.x), Mathf.Round(zeroBasedPosition.y),
            Mathf.Round(zeroBasedPosition.z));
    }

    public void RemoveAndDestroyBlock(Block block)
    {
        RemoveBlockAt(block.GetGridPosition());
        block.DestroySelf();
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
            new Vector3(-1, positionY, middle ? -1 : 0),
            new Vector3(-1, positionY, middle ? 0 : 1),

            new Vector3(0, positionY, -1),
            new Vector3(0, positionY, 1),

            new Vector3(1, positionY, middle ? -1 : 0),
            new Vector3(1, positionY, middle ? 0 : 1),
        };
        var nearbyPositions = translations
            .Select(translation => position + translation)
            .Where(IsWithinBounds)
            .Select(newPosition =>
            {
                return new Vector3(
                    newPosition.x,
                    GetStackHeight(newPosition),
                    newPosition.z
                );
            });

        return nearbyPositions;
    }

    public List<Vector3> GetNearbyEmptyPositions(Vector3 position)
    {
        return GetNeighbouringPositions(position)
            .Where(newPosition => !blocksRepository.HasAtPosition(newPosition))
            .ToList();
    }

    public IEnumerable<Vector3> GetNearbyEmptyPositionsStream(Vector3 position)
    {
        return GetNeighbouringPositions(position)
            .Where(newPosition => !blocksRepository.HasAtPosition(newPosition));
    }

    public bool IsBlockLowestWater(Block block)
    {
        if (!block.IsWater()) return false;

        var position = block.GetGridPosition();

        var lowestWater = GetStack(position).First(otherBlock => otherBlock.IsWater());
        var blockHasSameLevelAsLowestWater = Math.Abs(lowestWater.GetGridPosition().y - position.y) < .5f;
        return blockHasSameLevelAsLowestWater;
    }

    public void AddBlockOnTopOf(Block blockToAdd, GameObject blockToAddRoot, Block blockAtBottom)
    {
        var gridPosition = blockAtBottom.GetGridPosition() + Vector3.up;
        blockToAdd.SetGridPosition(gridPosition);
        blocksRepository.SetAtPosition(blockToAdd, gridPosition);
        MakeBlockMoreUnique(blockToAdd);

        blockAtBottom.PlaceOnTop(blockToAdd, blockToAddRoot);
    }

    public void AddAndPositionBlock(Block block, Vector3 gridPosition)
    {
        blocksRepository.SetAtPosition(block, gridPosition);
        MakeBlockMoreUnique(block);

        PositionBlock(block, gridPosition);
    }

    private void PositionBlock(Block block, Vector3 gridPosition)
    {
        block.SetGridPosition(gridPosition);
        block.transform.position =
            ToRealCoordinates(
                gridPosition); // TODO Should fix so that the block.BlockRoot() is the one being moved, but right now there is a weird bug when doing that...
    }

    private void MakeBlockMoreUnique(Block block)
    {
        block.RandomRotateAlongY();
    }

    public void ReplaceBlock(Block toBeReplaced, Block replacement)
    {
        var gridPosition = toBeReplaced.GetGridPosition();
        RemoveAndDestroyBlock(toBeReplaced);

        var blockBeneathPosition = gridPosition - Vector3.up;
        var blockBeneath = blocksRepository.GetMaybeAtPosition(blockBeneathPosition);
        if (blockBeneath)
        {
            AddBlockOnTopOf(replacement, replacement.transform.parent.gameObject, blockBeneath);
        }
        else
        {
            AddAndPositionBlock(replacement, gridPosition);
        }
    }

    public List<Block> GetWaterBlocks()
    {
        return blocksRepository.StreamBlocks()
            .Where(b => b.blockType == Block.BlockType.Lake)
            .ToList();
    }

    public IEnumerable<Block> GetStableShorelineBlocks()
    {
        return blocksRepository.StreamBlocks()
            .Where(b => b.blockType == Block.BlockType.Lake && b.IsStable() && !b.IsWaterLocked());
    }

    public IEnumerable<Block> GetWaterBlocksStream()
    {
        return blocksRepository.StreamBlocks()
            .Where(b => b.blockType == Block.BlockType.Lake);
    }

    public IEnumerable<Block> GetNearbyBlocksWithinRange(Vector3 blockGridPosition, float radius)
    {
        var realCenterPosition = ToRealCoordinates(blockGridPosition);
        var sizeOfBlock = blockTemplate.transform.localScale.x;

        var hits = Physics.OverlapSphere(realCenterPosition, sizeOfBlock * radius * .75f);

        return hits
            .Select(hit =>
            {
                var blockRigidbody = hit.attachedRigidbody;
                
                return blockRigidbody ? blockRigidbody.gameObject.GetComponent<Block>() : null;
            })
            .Where(block => block != null);
    }

    public IEnumerable<Block> GetNearbyBlocks(Vector3 position)
    {
        return GetNeighbouringPositions(position)
            .Where(newPosition => blocksRepository.HasAtPosition(newPosition))
            .Select(newPosition => blocksRepository.GetAtPosition(newPosition));
    }
    
    public IEnumerable<Block> GetNearbyLevelBlocks(Vector3 position)
    {
        return GetNeighbouringPositions(position)
            .Select(p => new Vector3(p.x, position.y, p.z))
            .Where(newPosition => blocksRepository.HasAtPosition(newPosition))
            .Select(newPosition => blocksRepository.GetAtPosition(newPosition));
    }

    public IEnumerable<Block> GetNearbyLotsStream(Vector3 position)
    {
        return GetNearbyBlocks(position)
            .Where(b => b.IsLot());
    }

    public List<Block> GetNearbyVacantLots(Vector3 position)
    {
        return GetNearbyLotsStream(position)
            .Where(block => block.IsVacant())
            .ToList();
    }

    public IEnumerable<Block> GetNearbyVacantLotsStream(Vector3 position)
    {
        return GetNearbyLotsStream(position)
            .Where(block => block.IsVacant());
    }

    public IEnumerable<Block> GetNearbyLand(Vector3 position)
    {
        return GetNearbyBlocks(position)
            .Where(b => b.IsLand());
    }

    public IEnumerable<Block> GetNearbyLandOrLake(Vector3 position)
    {
        return GetNearbyBlocks(position)
            .Where(b => b.IsLand() || b.IsLake());
    }

    public List<Block> GetVacantBlocks()
    {
        return blocksRepository.StreamPairs()
            .Where(pair => pair.Value.IsVacant())
            .Select(pair => pair.Value)
            .ToList();
    }
    
    public IEnumerable<KeyValuePair<Vector3, Block>> GetAllTopLots()
    {
        return blocksRepository.StreamPairs()
            .Where(pair => pair.Value.IsTopBlockInStack() && pair.Value.IsLand());
    }

    public IEnumerable<Block> GetVacantBlocksStream()
    {
        return blocksRepository.StreamPairs()
            .Where(pair => pair.Value.IsVacant())
            .Select(pair => pair.Value);
    }

    public List<Block> GetBlocksWithHouses()
    {
        return blocksRepository.StreamBlocks().Where(block => block.OccupiedByHouse()).ToList();
    }

    public IEnumerable<Block> GetBlocksWithHousesStream()
    {
        return blocksRepository.StreamBlocks().Where(block => block.OccupiedByHouse());
    }

    public IEnumerable<Block> GetBlocksWithDesertHouses()
    {
        return blocksRepository.StreamBlocks().Where(block => block.OccupiedByDesertHouse());
    }

    public int
        GetStackHeight(
            Vector3 position)
    {
        var block = blocksRepository.GetAtPosition(
            new Vector3(
                position.x,
                Block.LowestLevel,
                position.z
            )
        );
        return Convert.ToInt32(block.GetTopBlock().GetGridPosition().y);
    }

    public Block GetBlockAtTopOfStack(Vector3 position)
    {
        int topBlockLevel = Convert.ToInt32(Block.LowestLevel);
        Block topBlock = blocksRepository.GetMaybeAtPosition(
            new Vector3(
                position.x,
                topBlockLevel,
                position.z
            )
        );

        topBlockLevel += 1;
        while (true)
        {
            var block = blocksRepository.GetMaybeAtPosition(
                new Vector3(
                    position.x,
                    topBlockLevel,
                    position.z
                )
            );
            if (block == null) break;

            topBlock = block;
            topBlockLevel++;
        }

        return topBlock;
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
            .Select(hit =>
            {
                var blockRigidbody = hit.attachedRigidbody;
                
                return blockRigidbody ? blockRigidbody.gameObject.GetComponent<Block>() : null;
            })
            .Where(block => block != null)
            .Sum(block =>
            {
                // if (block.OccupiedByHouse() && block.GetOccupantHouse().IsBig()) return -10;
                if (block.OccupiedByHouse()) return -1;

                var blockHeight = block.GetGridPosition().y;

                if (block.OccupiedByGreens())
                {
                    var greens = block.GetOccupantGreens();
                    if (greens.IsGrown())
                    {
                        if (blockHeight > 10) return 20;
                        if (blockHeight > 4) return 10;
                        if (blockHeight > 2) return 6;
                        if (blockHeight > 0) return 4;

                        return 2;
                    }
                }

                if (block.IsGrass() && blockHeight > 10) return 4;
                if (block.IsGrass() && blockHeight > 4) return 2;
                if (block.IsGrass() && blockHeight > 2) return 1;
                if (block.IsGrass() && blockHeight > 0) return .5f;

                if (block.IsWater() && blockHeight > 10) return 4;
                if (block.IsWater() && blockHeight > 4) return 4;
                if (block.IsWater() && blockHeight > 2) return 2;

                if (block.IsSand()) return -1;

                return 0;
            });
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
                return new Vector2(32, 32);
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
                    var isMiddle = Math.Abs(row % 2) < .5f;

                    var columnHeight = Mathf.Ceil(dimensions.x / xChunks);

                    var xStart = columnHeight * (xChunk - 1);
                    var xMax = columnHeight * xChunk;
                    for (var column = Math.Abs(xStart) < .5f ? -1 : xStart; column < xMax; column++)
                    {
                        Block previousBlock = null;

                        for (var level = Block.LowestLevel; level <= 0; level++)
                        {
                            if (isMiddle && Math.Abs(column - (-1)) < .5f) continue;

                            var blockToUse = BlockFactory.Get().grassBlockTemplate;
                            if (level == 0)
                            {
                                var neededLeftColumn = isMiddle ? 0 : -1;
                                var neededRightColumn = dimensions.x - 1;
                                
                                // Fill outmost border with water
                                if (column == neededLeftColumn || column == neededRightColumn || row == 0 ||
                                    row == dimensions.y - 1)
                                {
                                    blockToUse = BlockFactory.Get().topWaterBlockTemplate;
                                }
                                else
                                {
                                    var point = new Vector2(column, row);
                                    var center = new Vector2(Mathf.Round(dimensions.x / 2), Mathf.Round(dimensions.y / 2));
                                    var distanceToCenter = Vector2.Distance(point, center);
                                    var radius = Mathf.Round(dimensions.x / 2);

                                    var distanceFactor = distanceToCenter / radius;
                                    // if (distanceFactor > .95f)
                                    // {
                                    //     blockToUse = BlockFactory.Get().topWaterBlockTemplate;
                                    // }
                                    // else if (distanceFactor > .7f)
                                    // {
                                    //     if (Random.value < .75f)
                                    //     {
                                    //         blockToUse = BlockFactory.Get().regularWaterBlockTemplate;
                                    //     }
                                    //     else
                                    //     {
                                    //         blockToUse = BlockFactory.Get().sandBlockTemplate;
                                    //     }
                                    // }
                                    // else if (distanceFactor > .6f)
                                    // {
                                    //     blockToUse = BlockFactory.Get().sandBlockTemplate;
                                    // }
                                    
                                    if (distanceFactor > .75f)
                                    {
                                        blockToUse = BlockFactory.Get().topWaterBlockTemplate;
                                    }
                                    else if (distanceFactor > .7f)
                                    {
                                        if (Random.value < .75f)
                                        {
                                            blockToUse = BlockFactory.Get().regularWaterBlockTemplate;
                                        }
                                        else
                                        {
                                            blockToUse = BlockFactory.Get().sandBlockTemplate;
                                        }
                                    }
                                    else if (distanceFactor > .6f)
                                    {
                                        blockToUse = BlockFactory.Get().sandBlockTemplate;
                                    }
                                }

                                // else if (WithinRange(column, neededLeftColumn + 1, neededLeftColumn + 3) ||
                                //          WithinRange(column, neededRightColumn - 3, neededRightColumn - 1) ||
                                //          WithinRange(row, 1, 3) ||
                                //          WithinRange(row, dimensions.y - 4, dimensions.y - 2))
                                // {
                                //     if (WithinRange(column, neededLeftColumn + 1, neededLeftColumn + 2) ||
                                //         WithinRange(column, neededRightColumn - 2, neededRightColumn - 1) ||
                                //         WithinRange(row, 1, 2) ||
                                //         WithinRange(row, dimensions.y - 3, dimensions.y - 2))
                                //     {
                                //         if (WithinRange(column, neededLeftColumn + 1, neededLeftColumn + 1) ||
                                //             WithinRange(column, neededRightColumn - 1, neededRightColumn - 1) ||
                                //             WithinRange(row, 1, 1) ||
                                //             WithinRange(row, dimensions.y - 2, dimensions.y - 2))
                                //         {
                                //             if (Random.value < .7)
                                //             {
                                //                 blockToUse = BlockFactory.Get().topWaterBlockTemplate;
                                //             }
                                //             else
                                //             {
                                //                 blockToUse = BlockFactory.Get().sandBlockTemplate;
                                //             }
                                //         }
                                //         else if (Random.value < .2)
                                //         {
                                //             blockToUse = BlockFactory.Get().topWaterBlockTemplate;
                                //         }
                                //         else
                                //         {
                                //             blockToUse = BlockFactory.Get().sandBlockTemplate;
                                //         }
                                //     }
                                //     else if (Random.value < .8f)
                                //     {
                                //         blockToUse = BlockFactory.Get().sandBlockTemplate;
                                //     }
                                // }
                            }

                            var blockPosition = new Vector3(row, level, column);
                            var blockObject = Instantiate(blockToUse);
                            var block = blockObject.GetComponentInChildren<Block>();

                            if (level == Block.LowestLevel)
                            {
                                AddAndPositionBlock(block, blockPosition);
                                previousBlock = block;
                            }
                            else
                            {
                                AddBlockOnTopOf(block, blockObject, previousBlock);
                                previousBlock = block;
                            }
                        }
                    }

                    yield return new WaitForEndOfFrame();
                }
            }
        }

        _doneGeneratingWorld = true;
    }

    private bool WithinRange(float value, float min, float max)
    {
        return value >= min && value <= max;
    }

    private bool IsWithinBounds(Vector3 point)
    {
        var row = point.x;
        var column = point.z;
        var isEven = Math.Abs(row % 2) < .5f;

        if (isEven)
        {
            return row >= _currentMinBound.x && row < _currentDimensions.x
                                             && column >= _currentMinBound.y && column < _currentDimensions.y;
        }
        else
        {
            return row >= _currentMinBound.x && row < _currentDimensions.x
                                             && column >= (_currentMinBound.y - 1) && column < _currentDimensions.y;
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
        return _topLeftPointTransform.position.z;
    }

    public float Right()
    {
        return _topLeftPointTransform.position.x;
    }

    public float Bottom()
    {
        return _topLeftPointTransform.position.z + blockTemplate.transform.localScale.z * _currentDimensions.y;
    }

    public float Left()
    {
        return _topLeftPointTransform.position.x +
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

    public IEnumerable<Block> GetBlocksWithOccupants()
    {
        return blocksRepository.StreamBlocks().Where(block => block.HasOccupant());
    }

    public IEnumerable<Block> GetBlocksWithDocks()
    {
        return blocksRepository
            .StreamBlocks()
            .Where(block => block.OccupiedByDocks());
    }

    public Block.BlockType GetMajorityBlockTypeWithinRange(Vector3 gridPosition, float range)
    {
        return GetNearbyBlocksWithinRange(gridPosition, range)
            .GroupBy(block => block.blockType)
            .OrderByDescending(group => group.Count())
            .First()
            .Key;
    }

    public bool NoNearby(Vector3 gridPosition, float radius, Func<Block, bool> filter)
    {
        var hasAnyNearby = GetNearbyBlocksWithinRange(gridPosition, radius)
            .Where(filter)
            .Any();

        return !hasAnyNearby;
    }

    public List<Biome> GetBiomes()
    {
        var biomeBuilders = new List<Biome.BiomeType>()
            {
                Biome.BiomeType.Forrest,
                Biome.BiomeType.Mountain,
                Biome.BiomeType.Plains,
                Biome.BiomeType.Shoreline
            }
            .Select(type => new BiomeBuilder(type))
            .ToList();

        foreach (var block in blocksRepository.StreamBlocks())
        {
            foreach (var biomeBuilder in biomeBuilders)
            {
                if (biomeBuilder.AnalyzeBlock(block)) break;
            }
        }

        return biomeBuilders.Select(builder => builder.Build()).ToList();
    }

    public IEnumerable<Block> GetSandBlocks()
    {
        return blocksRepository.StreamBlocks().Where(b => b.blockType == Block.BlockType.Sand);
    }

    public bool BlockCanBeReplacedBySandBlock(Block block)
    {
        if (block.IsSand())
        {
            if (block.OccupiedByDesertHouse()) return false;
            if (block.OccupiedByGreens())
            {
                return !HasNearbyGreens(block.GetGridPosition());
            }
        }
        else if (block.IsWater())
        {
            return !HasNearbyWater(block.GetGridPosition());
        }
        else if (block.IsOutsideWater())
        {
            return false;
        }

        return true;
    }

    private bool HasNearbyGreens(Vector3 gridPosition)
    {
        return GetNearbyBlocks(gridPosition)
            .Any(otherBlock => otherBlock.OccupiedByGreens());
    }

    private bool HasNearbyWater(Vector3 gridPosition)
    {
        var nearbyWaterBlocks = GetNearbyBlocksWithinRange(gridPosition, 2)
            .Count(block => block.IsWater());
        return nearbyWaterBlocks > waterLimit;
    }

    public IEnumerable<Block> GetBlocksWithShrines()
    {
        return blocksRepository.StreamBlocks().Where(block => block.OccupiedByShrine());
    }
}