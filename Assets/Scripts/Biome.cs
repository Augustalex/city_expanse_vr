using System.Collections.Generic;
using System.Linq;

public class Biome
{
    private List<Block> _blocks;
    private BiomeType _type;

    public enum BiomeType
    {
        Plains,
        Forrest,
        Sand,
        Mountain,
        Shoreline
    }

    public Biome(BiomeType type, List<Block> blocks)
    {
        _blocks = blocks;
        _type = type;
    }

    public int Size()
    {
        return _blocks.Count;
    }

    public BiomeType Type()
    {
        return _type;
    }

    public List<Block> GetBlocks()
    {
        return _blocks;
    }
}