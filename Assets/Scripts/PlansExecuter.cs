using System;
using UnityEngine;

public class PlansExecuter : MonoBehaviour
{
    private City _city;

    void Start()
    {
        _city = City.Get();
    }
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            _city.SpawnOneHouse();
        }
    }
}