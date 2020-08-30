using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NatureScore : MonoBehaviour
{
    public WorldPlane worldPlane;

    private double _lastCaluclated = -999;

    void Update()
    {
        // var delta = Time.fixedTime - _lastCaluclated;
        // if (delta > 2)
        // {
        //     GetComponent<Text>().text = "Nature score: " + worldPlane.NatureScore(new Vector3(0,0,0), 1000).ToString(); 
        //     _lastCaluclated = Time.fixedTime;
        // }
    }
}
