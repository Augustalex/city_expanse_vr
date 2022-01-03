using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class MountainousLandWorldGenerator : WorldGenerator
{
    public MountainousLandWorldGenerator(IWorldGeneratorUnityInterface unityInterface) : base(unityInterface)
    {
    }

    public override IEnumerator Create(Vector2 dimensions)
    {
        yield return CreatePlains(dimensions);
        yield return CreateMountain2(dimensions, new Vector2(Mathf.Round(dimensions.x / 2), Mathf.Round(dimensions.y / 2)), Random.Range(3,8));
        yield return CreateMountain2(dimensions, new Vector2(Mathf.Round(dimensions.x / 2) + (Random.Range(0, 12) - 6), Mathf.Round(dimensions.y / 2) + (Random.Range(0, 12) - 6)), Random.Range(2,6));
        yield return CreateMountain2(dimensions, new Vector2(Mathf.Round(dimensions.x / 2) + (Random.Range(0, 12) - 6), Mathf.Round(dimensions.y / 2) + (Random.Range(0, 12) - 6)), Random.Range(1,5));
    }

    public IEnumerator CreatePlains(Vector2 dimensions)
    {
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
                                    var center = new Vector2(Mathf.Round(dimensions.x / 2),
                                        Mathf.Round(dimensions.y / 2));
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
                                //          WithinRange(row, _dimensions.y - 4, _dimensions.y - 2))
                                // {
                                //     if (WithinRange(column, neededLeftColumn + 1, neededLeftColumn + 2) ||
                                //         WithinRange(column, neededRightColumn - 2, neededRightColumn - 1) ||
                                //         WithinRange(row, 1, 2) ||
                                //         WithinRange(row, _dimensions.y - 3, _dimensions.y - 2))
                                //     {
                                //         if (WithinRange(column, neededLeftColumn + 1, neededLeftColumn + 1) ||
                                //             WithinRange(column, neededRightColumn - 1, neededRightColumn - 1) ||
                                //             WithinRange(row, 1, 1) ||
                                //             WithinRange(row, _dimensions.y - 2, _dimensions.y - 2))
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
                            var blockObject = UnityInterface.Create(blockToUse);
                            var block = blockObject.GetComponentInChildren<Block>();

                            if (level == Block.LowestLevel)
                            {
                                UnityInterface.AddAndPositionBlock(block, blockPosition);
                                previousBlock = block;
                            }
                            else
                            {
                                UnityInterface.AddBlockOnTopOf(block, blockObject, previousBlock);
                                previousBlock = block;
                            }
                        }
                    }

                    yield return new WaitForEndOfFrame();
                }
            }
        }
    }

    public IEnumerator CreateMountain2(Vector2 dimensions, Vector2 center, int height)
    {
        var rowHeight = Mathf.Ceil(dimensions.y);
        var yStart = 0;
        var yMax = rowHeight;
        for (var row = yStart; row < yMax; row++)
        {
            var isMiddle = Math.Abs(row % 2) < .5f;

            var columnHeight = Mathf.Ceil(dimensions.x);

            var xStart = 0;
            var xMax = columnHeight;
            for (var column = Math.Abs(xStart) < .5f ? -1 : xStart; column < xMax; column++)
            {
                for (var level = Block.LowestLevel; level <= 0; level++)
                {
                    if (isMiddle && Math.Abs(column - (-1)) < .5f) continue;

                    if (level == 0)
                    {
                        var point = new Vector2(column, row);
                        // var center = new Vector2(Mathf.Round(dimensions.x / 2), Mathf.Round(dimensions.y / 2));
                        var distanceToCenter = Vector2.Distance(point, center);
                        var radius = Mathf.Round(dimensions.x / 2);

                        var distanceFactor = distanceToCenter / radius;
                        if (distanceFactor < .5f)
                        {
                            var normalizedFactor = distanceFactor / .5f;
                            var easedFactor = EaseInOutBounce(1 - normalizedFactor);
                            FillPoint(point, height * (easedFactor));
                        }
                    }
                }
            }

            yield return new WaitForEndOfFrame();
        }
    }

    private void FillPoint(Vector2 point, float height)
    {
        Vector3 midpoint = new Vector3(point.x, Mathf.Round(height), point.y);

        var grassBlockTemplate = BlockFactory.Get().grassBlockTemplate;

        var previousBlock = UnityInterface.GetBlockAtTopOfStack(midpoint);
        for (int i = 1; i < midpoint.y; i++)
        {
            var blockObject = UnityInterface.Create(grassBlockTemplate);
            var block = blockObject.GetComponentInChildren<Block>();
            UnityInterface.AddBlockOnTopOf(block, blockObject, previousBlock);
            previousBlock = block;
        }
    }
    
    private float EaseOutExpo(float x) {
        return x == 1 ? 1 : 1 - Mathf.Pow(2, -10 * x);;
    }
    
    private float EaseInExpo(float x) {
        return x == 0 ? 0 : Mathf.Pow(2, 10 * x - 10);
    }
    
    private float EaseInOutBounce(float x) {
        return x < 0.5
            ? (1 - EaseOutBounce(1 - 2 * x)) / 2
            : (1 + EaseOutBounce(2 * x - 1)) / 2;
    }
    
    private float EaseOutBounce(float x) {
        var n1 = 7.5625f;
        var d1 = 2.75f;

        if (x < 1f / d1) {
            return n1 * x * x;
        } else if (x < 2f / d1) {
            return n1 * (x -= 1.5f / d1) * x + 0.75f;
        } else if (x < 2.5f / d1) {
            return n1 * (x -= 2.25f / d1) * x + 0.9375f;
        } else {
            return n1 * (x -= 2.625f / d1) * x + 0.984375f;
        }
    }
}