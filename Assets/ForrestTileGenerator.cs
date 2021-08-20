using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class ForrestTileGenerator : MonoBehaviour
{
    public GameObject[] treeTemplates;
    public GameObject[] bushTemplates;
    public GameObject[] rockTemplates;

    void Start()
    {
        var treeCount = Random.Range(1, 2);
        for (int i = 0; i < treeCount; i++)
        {
            var randomTree = treeTemplates[Random.Range(0, treeTemplates.Length)];
            var tree = Instantiate(randomTree, Vector3.zero, Quaternion.identity, transform);

            var randomDirection = Random.insideUnitCircle;
            var randomDirection3D = new Vector3(randomDirection.x, 0, randomDirection.y);
            var newRandomPosition = randomDirection3D * .4f;
            tree.transform.localPosition = new Vector3(newRandomPosition.x, 0, newRandomPosition.z);

            var size = Random.Range(1.5f, 3f);
            tree.transform.localScale = new Vector3(size, size, size);

            var currentRotation = tree.transform.rotation;
            var randomRotation = Random.rotation.eulerAngles;
            tree.transform.rotation = Quaternion.Euler(
                currentRotation.x,
                randomRotation.y,
                currentRotation.z
            );
        }
        
        var bushCount = Random.Range(1, 3);
        for (int i = 0; i < bushCount; i++)
        {
            var randomBush = bushTemplates[Random.Range(0, bushTemplates.Length)];
            var bush = Instantiate(randomBush, transform, false);
        
            var randomDirection = Random.insideUnitCircle;
            var randomDirection3D = new Vector3(randomDirection.x, 0, randomDirection.y);
            var newRandomPosition = randomDirection3D * .1f;
            bush.transform.localPosition = new Vector3(newRandomPosition.x, -0.138f, newRandomPosition.z);
            // bush.transform.localScale = new Vector3(1, 1, 1);
            
            var currentRotation = bush.transform.rotation;
            var randomRotation = Random.rotation.eulerAngles;
            bush.transform.rotation = Quaternion.Euler(
                currentRotation.x,
                randomRotation.y,
                currentRotation.z
            );
        }

        var rocksCount = Random.Range(1, 3);
        for (int i = 0; i < rocksCount; i++)
        {
            var randomRock = rockTemplates[Random.Range(0, rockTemplates.Length)];
            var rock = Instantiate(randomRock, transform, false);

            var randomDirection = Random.insideUnitCircle;
            var randomDirection3D = new Vector3(randomDirection.x, 0, randomDirection.y);
            var newRandomPosition = randomDirection3D * .15f;
            rock.transform.localPosition = new Vector3(newRandomPosition.x, 0, newRandomPosition.z);
            
            var currentRotation = rock.transform.rotation;
            var randomRotation = Random.rotation.eulerAngles;
            rock.transform.rotation = Quaternion.Euler(
                currentRotation.x,
                randomRotation.y,
                currentRotation.z
            );
        }
    }
}