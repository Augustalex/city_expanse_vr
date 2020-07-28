using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HouseSpawn : MonoBehaviour
{
    public GameObject[] tinyHouseTemplates;
    public GameObject[] largeHouseTemplates;

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
        Destroy(_activeHouse);
        
        var houseTemplate = largeHouseTemplates[Random.Range(0, largeHouseTemplates.Length)];
        var house = Instantiate(houseTemplate);
        house.transform.SetParent(transform, false);

        _size = 1;
    }

    public bool IsLarge()
    {
        return _size == 1;
    }
}
