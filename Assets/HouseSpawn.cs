using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HouseSpawn : MonoBehaviour
{
    public GameObject[] tinyHouseTemplates;
    public GameObject[] largeHouseTemplates;
    public GameObject[] megaHouseTemplates;

    private GameObject _activeHouse;
    private int _size;

    void Awake()
    {
        SetupHouse(0, tinyHouseTemplates);
    }

    public void Upgrade()
    {
        if (_size == 0)
        {
            SetupHouse(1, largeHouseTemplates);
        }
        else if (_size == 1)
        {
            SetupHouse(2, megaHouseTemplates);
        }
    }

    private void SetupHouse(int size, GameObject[] templates)
    {
        Destroy(_activeHouse);

        var houseTemplate = templates[Random.Range(0, templates.Length)];
        var house = Instantiate(houseTemplate, transform, false);
                    
        _activeHouse = house;
        _size = size;
    }

    public bool IsSmall()
    {
        return _size == 0;
    }
    
    public bool IsBig()
    {
        return _size == 1;
    }
    
    public bool IsMegaBig()
    {
        return _size == 2;
    }

    public void SetToBig()
    {
        SetupHouse(1, largeHouseTemplates);
    }

    public void SetToMegaBig()
    {
        SetupHouse(2, megaHouseTemplates);
    }
}