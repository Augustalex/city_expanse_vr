using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DayCycle : MonoBehaviour
{
    public GameObject sun;
    public GameObject pivotPoint;
    public GameObject sunLightSource;

    public float time = 0;

    private Vector3 _startPosition;
    private Quaternion _startRotation;

    void Start()
    {
        RotateSun();
        SetSunLightSourcePositionAndRotation();

        _startPosition = sun.transform.position;
        _startRotation = sun.transform.rotation;
    }

    void Update()
    {
        RotateSun();
        SetSunLightSourcePositionAndRotation();

        time += Time.deltaTime;

        if (time > 100f)
        {
            var block = WorldPlane.Get().blocksRepository.GetAtPosition(new Vector3(10, 0, 10));
            SendMeteorBlockInteractor meteorSender = GameObject.FindObjectOfType<SendMeteorBlockInteractor>();

            var target = block.gameObject;
            if (meteorSender.Interactable(target))
            {
                meteorSender.Interact(target, () =>
                {
                    time = 0;
                    ResetSun();
                });
            }
        }
    }

    private void SetSunLightSourcePositionAndRotation()
    {
        sunLightSource.transform.position = sun.transform.position;
        sunLightSource.transform.LookAt(pivotPoint.transform);
    }

    private void RotateSun()
    {
        sun.transform.RotateAround(pivotPoint.transform.position, Vector3.forward + Vector3.left * .5f, Time.deltaTime * 10f);
    }

    private void ResetSun()
    {
        sun.transform.position = _startPosition;
        sun.transform.rotation = _startRotation;
    }
}