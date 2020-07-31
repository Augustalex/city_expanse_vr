using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Oculus.Platform;
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
                var block = blockObject.GetComponentInChildren<Block>();

                AddBlockToPosition(block, blockPosition);
            }
        }
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

    public void RemoveBlockAt(Vector3 position)
    {
        _blocks.Remove(position);
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
        block.RandomRotateAlongY();

        _blocks[position] = block;

        MakeSureTopGrassBlocksHaveCorrectTexture(position);
    }

    private void MakeSureTopGrassBlocksHaveCorrectTexture(Vector3 position)
    {
        var stack = _blocks.Where(pair => pair.Key.x == position.x && pair.Key.z == position.z);

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
        return _blocks.Values
            .Where(b => b.blockType == Block.BlockType.Water)
            .ToList();
    }

    public List<Block> GetNearbyLots(Vector3 position)
    {
        return GetNeighbouringPositions(position)
            .Where(newPosition => _blocks.ContainsKey(newPosition))
            .Select(newPosition => _blocks[newPosition])
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
        return _blocks
            .Where(pair => pair.Value.IsVacant())
            .Select(pair => pair.Value)
            .ToList();
    }

    public List<Block> GetBlocksWithHouses()
    {
        return _blocks.Values.Where(block => block.OccupiedByHouse()).ToList();
    }

    public int GetStackHeight(Vector3 position)
    {
        var stack = _blocks.Where(pair => pair.Key.x == position.x && pair.Key.z == position.z);

        var list = stack.ToList();
        if (list.Count == 0) return 0;

        return Convert.ToInt32(list.Max(pair => pair.Key.y));
    }

    public List<Block> GetStack(Vector3 position)
    {
        var stack = _blocks.Where(pair => pair.Key.x == position.x && pair.Key.z == position.z);

        return stack.Select(pair => pair.Value).ToList();
    }

    public int NatureScore()
    {
        return _blocks.Values.Sum(block =>
        {
            if (block.IsWater() && block.GetPosition().y > 4) return 4;
            if (block.IsWater() && block.GetPosition().y > 2) return 2;
            if (block.IsWater()) return 1;

            if (block.OccupiedByGreens() && block.GetPosition().y > 2) return 4;
            if (block.OccupiedByGreens()) return 2;

            if (block.OccupiedByHouse() && block.GetOccupantHouse().IsMegaBig()) return -100;
            if (block.OccupiedByHouse() && block.GetOccupantHouse().IsBig()) return -20;
            if (block.OccupiedByHouse() && block.GetOccupantHouse()) return -1;

            if (block.GetPosition().y > 4) return 2;
            if (block.GetPosition().y > 2) return 1;

            return 0;
        });
    }
}