using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoatFloat : MonoBehaviour
{
    public float amplitude = .0008f;
    public float frequency = .2f;

    private float _originalPositionY = 0f;
    
    void Start()
    {
        _originalPositionY = transform.position.y;
    }

    void Update()
    {
        var originalPosition = transform.position;
        transform.position = new Vector3(
            originalPosition.x,
            _originalPositionY - BoatYPosition(Time.fixedTime, amplitude, frequency),
            originalPosition.z
        );
    }

    float BoatYPosition(float time, float amplitudeScale, float frequencyScale)
    {
        //a=2.9
        //sin(x)*4+sin(2 (x-a))
        var x = time * frequencyScale;
        return (FirstWave(x) + SecondWave(x)) * amplitudeScale;
    }

    private static float FirstWave(float time)
    {
        return Mathf.Sin(3.2f * Mathf.PI * time) * 4;
    }

    private static float SecondWave(float time)
    {
        return Mathf.Sin(1.8f * Mathf.PI * (time - 2.9f));
    }
}