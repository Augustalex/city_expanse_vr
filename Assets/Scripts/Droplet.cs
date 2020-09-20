using System;
using System.Collections;
using UnityEngine;

public class Droplet : MonoBehaviour
{
    private void Update()
    {
        StartCoroutine(DestroySoon());
    }

    private IEnumerator DestroySoon()
    {
        yield return new WaitForSeconds(5);
        if (gameObject != null)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        var farmSpawn = other.GetComponent<FarmSpawn>();
        if (farmSpawn && !farmSpawn.IsGrown())
        {
            farmSpawn.Grow();
            Destroy(gameObject);
        }
        else
        {
            var greenSpawn = other.GetComponent<GreensSpawn>();
            if (greenSpawn && !greenSpawn.IsGrown())
            {
                greenSpawn.Grow();
                Destroy(gameObject);
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