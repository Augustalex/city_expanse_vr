using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ConfigurableWobblyText : MonoBehaviour
{
    public TMP_Text textComponent;
    public float frequency = 2f;
    public float waviness = 10f;
    public float amplitude = 5f;
    
    void Start()
    {
        textComponent = GetComponent<TMP_Text>();
    }

    void Update()
    {
        textComponent.ForceMeshUpdate();
        var textInfo = textComponent.textInfo;

        for (int i = 0; i < textInfo.characterCount; i++)
        {
            var charInfo = textInfo.characterInfo[i];

            if (!charInfo.isVisible)
            {
                continue;
            }

            var vertices = textInfo.meshInfo[charInfo.materialReferenceIndex].vertices;

            for (int j = 0; j < 4; j++)
            {
                var origin = vertices[charInfo.vertexIndex + j];
                vertices[charInfo.vertexIndex + j] =
                    origin + new Vector3(0, Mathf.Sin(Time.time * frequency + origin.x * waviness) * amplitude, 0);
            }
        }

        for (int i = 0; i < textInfo.meshInfo.Length; i++)
        {
            var meshInfo = textInfo.meshInfo[i];
            meshInfo.mesh.vertices = meshInfo.vertices;
            textComponent.UpdateGeometry(meshInfo.mesh, i);
        }
    }
}