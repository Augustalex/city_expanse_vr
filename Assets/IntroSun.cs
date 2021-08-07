using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroSun : MonoBehaviour
{
    private bool _started;
    private float _startedAt;
    private Color _ambientColor;
    private Color _targetColor;

    void Awake()
    {
        _ambientColor = RenderSettings.ambientLight;
        _targetColor = Color.black;
        
        RenderSettings.skybox.SetFloat("_Exposure", 2f);
        RenderSettings.sun.intensity = 1f;

        
        RenderSettings.ambientLight = _ambientColor;
        // RenderSettings.ambientIntensity = 2f;
        
        RenderSettings.reflectionIntensity = 1f;
        DynamicGI.UpdateEnvironment();
    }

    void Update()
    {
        if (_started)
        {
            transform.position += new Vector3(0, -1, 0) * Time.deltaTime * .4f;

            var elapsedTime = Time.fixedTime - _startedAt;
            var currentAlpha = Mathf.Lerp(2f, 0f, Mathf.Clamp01(elapsedTime / 9f));
            RenderSettings.skybox.SetFloat("_Exposure", currentAlpha);
            
            RenderSettings.reflectionIntensity =
                Mathf.Max(.05f, RenderSettings.reflectionIntensity - .1f * Time.deltaTime);

            RenderSettings.ambientLight = Color.Lerp(_ambientColor, _targetColor, elapsedTime / 9f);
            // RenderSettings.ambientIntensity =
                // Mathf.Max(.1f, RenderSettings.ambientIntensity - .2f * Time.deltaTime);
            
                
            RenderSettings.sun.intensity =
                Mathf.Max(.05f, RenderSettings.sun.intensity - .1f * Time.deltaTime);

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