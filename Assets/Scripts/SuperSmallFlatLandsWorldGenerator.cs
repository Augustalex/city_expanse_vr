using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class SuperSmallFlatLandsWorldGenerator : WorldGenerator
{
    public SuperSmallFlatLandsWorldGenerator(IWorldGeneratorUnityInterface unityInterface) : base(unityInterface)
    {
    }
    
    public override IEnumerator Create(Vector2 dimensions)
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
                                    var center = new Vector2(Mathf.Round(dimensions.x / 2), Mathf.Round(dimensions.y / 2));
                                    var distanceToCenter = Vector2.Distance(point, center);
                                    var radius = Mathf.Round(dimensions.x / 2);

                                    var distanceFactor = distanceToCenter / radius;

                                    if (distanceFactor > .4f)
                                    {
                                        blockToUse = BlockFactory.Get().topWaterBlockTemplate;
                                    }
                                    else if (distanceFactor > .3f)
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
                                    else if (distanceFactor > .2f)
                                    {
                                        blockToUse = BlockFactory.Get().sandBlockTemplate;
                                    }
                                }
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
}