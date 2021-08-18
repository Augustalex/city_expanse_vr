using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class ForrestTileGenerator : MonoBehaviour
{
    public GameObject[] treeTemplates;

    void Start()
    {
        var treeCount = Random.Range(1, 3);

        for (int i = 0; i < treeCount; i++)
        {
            var randomTree = treeTemplates[Random.Range(0, treeTemplates.Length)];
            var tree = Instantiate(randomTree, Vector3.zero, Quaternion.identity, transform);

            var randomDirection = Random.insideUnitCircle;
            var randomDirection3D = new Vector3(randomDirection.x, 0, randomDirection.y);
            var newRandomPosition = randomDirection3D * .4f;
            tree.transform.localPosition = new Vector3(newRandomPosition.x, 0, newRandomPosition.z);

            var currentRotation = tree.transform.rotation;
            var randomRotation = Random.rotation.eulerAngles;
            tree.transform.rotation = Quaternion.Euler(
                currentRotation.x,
                randomRotation.y,
                currentRotation.z
            );
        }
    }
}