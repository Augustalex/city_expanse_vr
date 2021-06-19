using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DayCycle : MonoBehaviour
{
    public GameObject sun;
    public GameObject pivotPoint;
    public GameObject sunLightSource;

    void Start()
    {
        
    }

    void Update()
    {
        sun.transform.RotateAround(pivotPoint.transform.position, Vector3.forward + Vector3.left, Time.deltaTime * 10f);
        sunLightSource.transform.position = sun.transform.position;
        sunLightSource.transform.LookAt(pivotPoint.transform);
    }
}
