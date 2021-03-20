using System;
using System.Collections;
using UnityEngine;

public class Droplet : MonoBehaviour
{
    private bool _consumed = false;

    private void OnTriggerEnter(Collider other)
    {
        if (_consumed) return;

        var farmSpawn = other.GetComponent<FarmSpawn>();
        if (farmSpawn && !farmSpawn.IsGrown())
        {
            farmSpawn.Grow();
            _consumed = true;
        }
        else
        {
            var greenSpawn = other.GetComponent<GreensSpawn>();
            if (greenSpawn && !greenSpawn.IsGrown())
            {
                greenSpawn.Grow();
                _consumed = true;
            }
            // else if (IsSand(other))
            // {
            // MakeSandToGrass(other);
            // }
        }
    }

    // private void MakeSandToGrass(Collider other)
    // {
    //     var block = other.GetComponent<Block>();
    //     var grassBlockRoot = BlockFactory.Get().GrassBlock();
    //     var grassBlock = grassBlockRoot.GetComponentInChildren<Block>();
    //     WorldPlane.Get().ReplaceBlock(block, grassBlock);
    //     Destroy(gameObject);
    // }
    //
    // private bool IsSand(Collider other)
    // {
    //     var block = other.GetComponent<Block>();
    //     return block.IsSand();
    // }
}