using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroSunSource : MonoBehaviour
{
    private Light _light;
    private bool _started;

    void Awake()
    {
        _light = GetComponent<Light>();
    }

    void Update()
    {
        if (_started)
        {
            // _light.intensity = Mathf.Max(0, _light.intensity - .3f * Time.deltaTime);
        }
    }

    public void StartSunSet()
    {
        _started = true;
    }
}