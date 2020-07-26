using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloodingWater : MonoBehaviour
{
    private Block _block;
    private WorldPlane _worldPlane;

    void Start()
    {
        _block = GetComponent<Block>();
        _worldPlane = GameObject.FindWithTag("WorldPlane").GetComponent<WorldPlane>();
    }

    void Update()
    {
        if (_block.IsPermaFrozen()) return;
        
        var nearbyEmptyPositions = _worldPlane.GetNearbyEmptyPositions(_block.GetPosition());
        foreach (var position in nearbyEmptyPositions)
        {
            var waterBlock = Replicate();
            _worldPlane.AddBlockToPosition(waterBlock, position);
            waterBlock.GetComponent<Block>().ShortFreeze();
        }
    }

    GameObject Replicate()
    {
        return Instantiate(gameObject);
    }
}