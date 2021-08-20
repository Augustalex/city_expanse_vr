using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterShaderController : MonoBehaviour
{
    public Material smoothShader;

    public Material stunningShader;

    public Material[] shaders;
    private int _shaderIndex;
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            var controlled = FindObjectsOfType<ControllerWaterShader>();
            foreach (var c in controlled)
            {
                c.GetComponent<MeshRenderer>().material = stunningShader;
            }
        }
        else if (Input.GetKeyDown(KeyCode.F2))
        {
            var controlled = FindObjectsOfType<ControllerWaterShader>();
            foreach (var c in controlled)
            {
                c.GetComponent<MeshRenderer>().material = smoothShader;
            }
        }
        else if (Input.GetKeyDown(KeyCode.F3))
        {
            var shader = shaders[_shaderIndex];

            _shaderIndex += 1;
            if (_shaderIndex >= shaders.Length) _shaderIndex = 0;
            
            var controlled = FindObjectsOfType<ControllerWaterShader>();
            foreach (var c in controlled)
            {
                c.GetComponent<MeshRenderer>().material = shader;
            }
        }
    }
}
