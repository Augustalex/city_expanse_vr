using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindTurbine : MonoBehaviour
{
    private WindController _windController;

    void Start()
    {
        _windController = WindController.Get();
    }

    void Update()
    {
        transform.rotation = _windController.GetWindDirection();
    }
}
