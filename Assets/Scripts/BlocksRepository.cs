﻿using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class BlocksRepository
{
    private Dictionary<Vector3, Block> _blocks = new Dictionary<Vector3, Block>();

    public void Remove(Vector3 position)
    {
        _blocks.Remove(position);
    }

    public bool HasAtPosition(Vector3 newPosition)
    {
        return _blocks.ContainsKey(newPosition);
    }

    public void SetAtPosition(Block block, Vector3 position)
    {
        _blocks[position] = block;
    }

    public IEnumerable<KeyValuePair<Vector3, Block>> StreamPairs()
    {
        return _blocks;
    }

    public IEnumerable<Block> StreamBlocks()
    {
        return _blocks.Values;
    }

    public Block GetAtPosition(Vector3 newPosition)
    {
        return _blocks[newPosition];
    }

    [CanBeNull]
    public Block GetMaybeAtPosition(Vector3 newPosition)
    {
        var foundBlock = _blocks.TryGetValue(newPosition, out var block);

        return foundBlock ? block : null;
    }

    public void RemoveAll()
    {
        _blocks.Clear();
    }
}