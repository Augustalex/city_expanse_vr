using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroSun : MonoBehaviour
{
    private bool _started;
    private float _startedAt;

    void Awake()
    {
        RenderSettings.skybox.SetFloat("_Exposure", 1.4f);
        RenderSettings.sun.intensity = 1f;
        RenderSettings.ambientIntensity = 1f;
        RenderSettings.reflectionIntensity = 1f;
        DynamicGI.UpdateEnvironment();
    }

    void Update()
    {
        if (_started)
        {
            transform.position += new Vector3(0, -1, 0) * Time.deltaTime * .12f;

            var elapsedTime = Time.fixedTime - _startedAt;
            var currentAlpha = Mathf.Lerp(1.4f, .2f, Mathf.Clamp01(elapsedTime / 10f));
            RenderSettings.skybox.SetFloat("_Exposure", currentAlpha);
            RenderSettings.reflectionIntensity =
                Mathf.Max(0, RenderSettings.reflectionIntensity - .07f * Time.deltaTime);
            RenderSettings.ambientIntensity =
                Mathf.Max(.1f, RenderSettings.ambientIntensity - .07f * Time.deltaTime);
            RenderSettings.sun.intensity =
                Mathf.Max(.1f, RenderSettings.sun.intensity - .07f * Time.deltaTime);
            DynamicGI.UpdateEnvironment();
        }
    }

    public void StartSunSet()
    {
        _started = true;
        _startedAt = Time.fixedTime;

        FindObjectOfType<IntroSunSource>().StartSunSet();
    }
}