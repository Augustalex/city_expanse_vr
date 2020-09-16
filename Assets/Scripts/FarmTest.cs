using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FarmTest : MonoBehaviour
{
    public GameObject grassBlockTemplate;
    public GameObject farmSpawnTemplate;
    private int _step;
    private Block _block;

    private void Start()
    {
        var _grassBlockParent = Instantiate(grassBlockTemplate);
        _block = _grassBlockParent.GetComponentInChildren<Block>();
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
            var farm = Instantiate(farmSpawnTemplate);
            _block.Occupy(farm);
        }
        else if (_step == 1)
        {
            _block.GetOccupant().GetComponent<FarmSpawn>().Grow();
        }

        _step += 1;
    }
}
