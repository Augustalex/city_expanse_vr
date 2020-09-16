using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FortTest : MonoBehaviour
{
    public GameObject grassBlockTemplate;
    public GameObject houseSpawnTemplate;
    private int _step;
    private Block _block;

    private void Start()
    {
        var grassBlockParent = Instantiate(grassBlockTemplate);
        _block = grassBlockParent.GetComponentInChildren<Block>();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            PerformTest();
        }
        else if (Input.GetMouseButtonDown(1))
        {
            TearDown();
        }
    }

    private void TearDown()
    {
        _block.DestroyOccupant();
        
        _step = 0;
    }

    private void PerformTest()
    {
        if (_step == 0)
        {
            var houseSpawn = Instantiate(houseSpawnTemplate);
            _block.Occupy(houseSpawn);
        }
        else if (_step == 1)
        {
            _block.GetOccupantHouse().Upgrade();
        }

        _step += 1;
    }
}
