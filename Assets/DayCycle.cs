using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DayCycle : MonoBehaviour
{
    public GameObject sun;
    public GameObject pivotPoint;
    public GameObject sunLightSource;
    public Transform sunDayPosition;
    public Transform sunDawnPosition;

    public float time = 0;

    private float _worldLifeLength = 60 * 5;

    void Start()
    {
        RotateSun();
        SetSunLightSourcePositionAndRotation();
        
        sun.transform.position = sunDayPosition.position;
        sun.transform.rotation = sunDayPosition.rotation;
    }

    void Update()
    {
        RotateSun();
        SetSunLightSourcePositionAndRotation();

        time += Time.deltaTime;

        if (time > _worldLifeLength)
        {
            var block = WorldPlane.Get().blocksRepository.GetAtPosition(new Vector3(10, 0, 10));
            SendMeteorBlockInteractor meteorSender = GameObject.FindObjectOfType<SendMeteorBlockInteractor>();

            var target = block.gameObject;
            if (meteorSender.Interactable(target))
            {
                meteorSender.Interact(target, () =>
                {
                    time = 0;
                    SetSunToDawn();
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

    private void SetSunToDawn()
    {
        sun.transform.position = sunDawnPosition.position;
        sun.transform.rotation = sunDawnPosition.rotation;
    }
}