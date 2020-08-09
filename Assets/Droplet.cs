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
            var block = other.GetComponent<Block>();
            if (block && block.IsSand())
            {
                var grassBlockRoot = BlockFactory.Get().GrassBlock();
                var grassBlock = grassBlockRoot.GetComponentInChildren<Block>();
                WorldPlane.Get().ReplaceBlock(block, grassBlock);
            }
        }
    }
}