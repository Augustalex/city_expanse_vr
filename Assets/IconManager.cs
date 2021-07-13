using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IconManager : MonoBehaviour
{
    private IconOff _iconOff;
    private IconOn _iconOn;

    void Start()
    {
        _iconOff = GetComponentInChildren<IconOff>();
        _iconOn = GetComponentInChildren<IconOn>();

        TurnOff();
    }
    
    public void TurnOn()
    {
        _iconOn.gameObject.SetActive(true);
        _iconOff.gameObject.SetActive(false);
    }

    public void TurnOff()
    {
        _iconOn.gameObject.SetActive(false);
        _iconOff.gameObject.SetActive(true);
    }
}
