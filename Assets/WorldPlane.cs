using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class WorldPlane : MonoBehaviour
{
    public GameObject blockTemplate;
    public Transform TopLeftPoint;

    private const int Width = 18;
    private const int Height = 18;

    private Dictionary<Vector3, Block> _blocks = new Dictionary<Vector3, Block>();

    void Start()
    {
        for (var x = 0; x < Height; x++)
        {
            for (var z = 0; z < Width; z++)
            {
                var blockPosition = new Vector3(x, 0, z);

                var blockObject = Instantiate(blockTemplate);
                
                blockObject.transform.position = ToRealCoordinates(blockPosition);
                var transformRotation = blockObject.transform.rotation;
                blockObject.transform.rotation = Quaternion.Euler(transformRotation.x, (Random.Range(0, 5) * 60), transformRotation.z);

                var block = blockObject.GetComponentInChildren<Block>();
                block.SetPosition(blockPosition);
                _blocks[blockPosition] = block;
            }
        }
    }

    public Vector3 ToRealCoordinates(Vector3 position)
    {
        var x = position.x;
        var z = position.z;
        var middle = Math.Abs(x % 2) < .5f;
        var xOffset = (((blockTemplate.transform.localScale.x + -0.014f) * x)) + blockTemplate.transform.localScale.x * .5f;
        var zOffset = blockTemplate.transform.localScale.z * z + blockTemplate.transform.localScale.z * .5f + (middle ? blockTemplate.transform.localScale.z * -.5f : 0);
        return TopLeftPoint.position + new Vector3(
                   xOffset,
                   blockTemplate.transform.localScale.y * .5f,
                   zOffset
               );
    }

    public void RemoveBlockAt(Vector3 position)
    {
        _blocks.Remove(position);
    }

    private IEnumerable<Vector3> GetNeighbouringPositions(Vector3 position)
    {
        var middle = Math.Abs(position.x % 2) < .5f;
        var translations = new List<Vector3>
        {
            new Vector3(0, position.y, -1),
            new Vector3(middle ? 1 : -1, position.y, -1),

            new Vector3(-1, position.y, 0),
            new Vector3(1, position.y, 0),
            
            new Vector3(0, position.y, 1),
            new Vector3(middle ? 1 : -1, position.y, 1),
        };
        var nearbyPositions = translations
            .Select(translation => position + translation)
            .Where(newPosition => !(newPosition.z < 0 || newPosition.z > (Width - 1) || newPosition.x < 0 ||
                                    newPosition.x > (Height - 1)));

        return nearbyPositions;
    }

    public List<Vector3> GetNearbyEmptyPositions(Vector3 position)
    {
        return GetNeighbouringPositions(position)
            .Where(newPosition => !_blocks.ContainsKey(newPosition))
            .ToList();
    }

    public void AddBlockToPosition(Block block, Vector3 position)
    {
        block.SetPosition(position);
        _blocks[position] = block;
    }

    public List<Block> GetWaterBlocks()
    {
        return _blocks.Values
            .Where(b => b.blockType == Block.BlockType.Water)
            .ToList();
    }

    public List<Block> GetNearbyVacantLots(Vector3 position)
    {
        return GetNeighbouringPositions(position)
            .Where(newPosition => _blocks.ContainsKey(newPosition))
            .Select(newPosition => _blocks[newPosition])
            .Where(b => b.blockType == Block.BlockType.Grass && b.IsVacant())
            .ToList();
    }
}