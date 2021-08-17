using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class WindController : MonoBehaviour
{
    private Quaternion _windDirection;
    private float _windForce = 10;
    private int _turningRate = 1;
    private static WindController _instance;

    void Awake()
    {
        _windDirection = Quaternion.Euler(0, Random.value, 0);
        _instance = this;
    }

    void Update()
    {
        if (Random.value < .0001f)
        {
            _turningRate = -_turningRate;
            _windForce = Random.Range(1, 5);
        }

        var turn = _turningRate * Time.deltaTime * _windForce;
        _windDirection = Quaternion.Euler(_windDirection.eulerAngles + new Vector3(0, turn, 0));
    }

    public Quaternion GetWindDirection()
    {
        return _windDirection;
    }

    public static WindController Get()
    {
        if (_instance == null)
        {
            throw new Exception("No WindController active in the scene while some object is trying to get it.");
        }

        return _instance;
    }
}