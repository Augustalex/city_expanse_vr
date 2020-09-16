using System.Collections.Generic;
using UnityEngine;

public class BlockHighlighter
{
    public static void Highlight(List<Block> blocks)
    {
        var worldPlane = WorldPlane.Get();
        foreach (var block in blocks)
        {
            worldPlane.RemoveAndDestroyBlock(block);
        }
    }
}