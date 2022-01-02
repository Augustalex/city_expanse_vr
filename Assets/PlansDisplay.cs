using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlansDisplay : MonoBehaviour
{
    private AvailablePlansTitle _title;

    void Start()
    {
        _title = GetComponentInChildren<AvailablePlansTitle>();
        
        _title.SetNoContentTitle();
    }
}
