using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterShaderController : MonoBehaviour
{
    public Material smoothShader;

    public Material stunningShader;

    public enum ShaderType
    {
        Smooth,
        Stunning
    };

    public ShaderType shaderType = ShaderType.Stunning;

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
    }
}
