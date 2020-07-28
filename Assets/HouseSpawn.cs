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
        var houseTemplate = tinyHouseTemplates[Random.Range(0, tinyHouseTemplates.Length)];
        var house = Instantiate(houseTemplate);
        house.transform.SetParent(transform, false);

        _activeHouse = house;
        _size = 0;
    }

    public void Upgrade()
    {
        if (_size == 0)
        {
            Destroy(_activeHouse);

            var houseTemplate = largeHouseTemplates[Random.Range(0, largeHouseTemplates.Length)];
            var house = Instantiate(houseTemplate);
            house.transform.SetParent(transform, false);

            _activeHouse = house;
            _size = 1;
        }
        else if (_size == 1)
        {
            Destroy(_activeHouse);

            var houseTemplate = megaHouseTemplates[Random.Range(0, megaHouseTemplates.Length)];
            var house = Instantiate(houseTemplate);
            house.transform.SetParent(transform, false);

            _activeHouse = house;
            _size = 2;
        }
    }

    public bool IsSmall()
    {
        return _size == 1;
    }
    
    public bool IsBig()
    {
        return _size == 1;
    }
    
    public bool IsMegaBig()
    {
        return _size == 2;
    }
}