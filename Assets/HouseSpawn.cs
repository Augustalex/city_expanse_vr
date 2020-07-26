using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HouseSpawn : MonoBehaviour
{
    public GameObject[] tinyHouseTemplates;

    void Awake()
    {
        var houseTemplate = tinyHouseTemplates[Random.Range(0, tinyHouseTemplates.Length)];
        var house = Instantiate(houseTemplate);
        house.transform.SetParent(transform, false);
    }
}
