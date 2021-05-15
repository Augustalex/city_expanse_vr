using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BiomeBuilder
{
    private Biome.BiomeType _type;
    private List<Block> _blocks = new List<Block>();

    public BiomeBuilder(Biome.BiomeType type)
    {
        _type = type;
    }

    public bool AnalyzeBlock(Block block)
    {
        var biome = FromBlock(block);
        if (biome != _type) return false;
        if (_blocks.Count > 0)
        {
            var blocksToAdd = _blocks
                .Where(biomeBlock =>
                {
                    var distance = Vector3.Distance(block.GetGridPosition(), biomeBlock.GetGridPosition());
                    return distance < 2;
                })
                .ToList();

            if (blocksToAdd.Count > 0)
            {
                _blocks.AddRange(
                    blocksToAdd
                );

                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            _blocks.Add(block);
            return true;
        }
    }

    public Biome.BiomeType FromBlock(Block block)
    {
        var type = block.blockType;
        if (type == Block.BlockType.Grass && block.GetGridPosition().y > 0)
        {
            return Biome.BiomeType.Mountain;
        }

        if (type == Block.BlockType.Grass)
        {
            return Biome.BiomeType.Plains;
        }

        if (type == Block.BlockType.Sand)
        {
            return Biome.BiomeType.Sand;
        }

        if (type == Block.BlockType.Lake)
        {
            return Biome.BiomeType.Shoreline;
        }

        return Biome.BiomeType.Plains;
    }

    public Biome Build()
    {
        return new Biome(_type, _blocks);
    }
}