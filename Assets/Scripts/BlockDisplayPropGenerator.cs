using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BlockDisplayPropGenerator : MonoBehaviour
{
    public GameObject blockTemplate;
    public GameObject primaryBlockTemplate;

    private enum SpaceType
    {
        Filler,
        Empty,
        Primary
    }

    void Start()
    {
        CreateWorld();
    }

    private void CreateWorld()
    {
        List<List<SpaceType>> grid = new List<List<SpaceType>>
        {
            new List<SpaceType> {SpaceType.Empty, SpaceType.Filler, SpaceType.Empty},
            new List<SpaceType> {SpaceType.Filler, SpaceType.Primary, SpaceType.Filler},
            new List<SpaceType> {SpaceType.Filler, SpaceType.Filler, SpaceType.Filler},
        };

        for (var rowIndex = 0; rowIndex < grid.Count; rowIndex++)
        {
            var row = grid[rowIndex];
            for (var columnIndex = 0; columnIndex < grid[0].Count; columnIndex++)
            {
                var spaceType = row[columnIndex];
                if (spaceType == SpaceType.Empty) continue;

                var blockObject = Instantiate(spaceType == SpaceType.Filler ? blockTemplate : primaryBlockTemplate, transform);
                var blockPosition = ToRealCoordinates(new Vector3(columnIndex, 0, rowIndex));
                blockObject.transform.localPosition = blockPosition;
            }
        }
    }

    public Vector3 ToRealCoordinates(Vector3 position)
    {
        var x = position.x;
        var z = position.z;
        var middle = Math.Abs(x % 2) < .5f;
        var xOffset = (((blockTemplate.transform.localScale.x + -0.114f) * x)) +
                      blockTemplate.transform.localScale.x * .5f;
        var zOffset = blockTemplate.transform.localScale.z * z + blockTemplate.transform.localScale.z * .5f +
                      (middle ? blockTemplate.transform.localScale.z * -.5f : 0);
        return new Vector3(
            xOffset,
            0,
            zOffset
        );
    }
}