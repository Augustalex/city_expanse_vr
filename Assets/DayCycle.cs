using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DayCycle : MonoBehaviour
{
    public GameObject sun;
    public GameObject pivotPoint;
    public Transform sunTarget;
    public GameObject sunLightSource;
    public Transform sunDayPosition;
    public Transform sunDawnPosition;

    public float time = 0;

    private float _worldLifeLength = 60 * 10;
    private float _sunSpeed = 1.5f;
    private bool _meteorSent;

    void Start()
    {
        // RotateSun();
        // SetSunLightSourcePositionAndRotation();

        // sun.transform.position = sunDayPosition.position;
        // sun.transform.rotation = sunDayPosition.rotation;
    }

    void Update()
    {
        // RotateSun();
        // SetSunLightSourcePositionAndRotation();

        time += Time.deltaTime;

        if (time > _worldLifeLength && !_meteorSent)
        {
            var block = WorldPlane.Get().blocksRepository.GetAtPosition(new Vector3(17, 0, 17));
            SendMeteorBlockInteractor meteorSender = GameObject.FindObjectOfType<SendMeteorBlockInteractor>();

            var target = block.gameObject;
            if (meteorSender.Interactable(target))
            {
                _meteorSent = true;
                meteorSender.Interact(target, () =>
                {
                    time = 0;
                    _meteorSent = false;
                    // SetSunToDawn();
                    // TODO END MENACING MUSIC and START REGULAR MUSIC
                });

                WorldPlane.Get().SetWorldIsEnding();
                // TODO TURN SKY RED
                // TODO START MENACING MUSIC
                // TODO ADD BURNING EFFECT TO METEOR
            }
        }
    }

    private void SetSunLightSourcePositionAndRotation()
    {
        sunLightSource.transform.position = sun.transform.position;
        sunLightSource.transform.LookAt(sunTarget.transform);
    }

    private void RotateSun()
    {
        var extraSpeed = sun.transform.position.y < -10f ? 50f : 0;
        var transformPosition = pivotPoint.transform.position;
        sun.transform.RotateAround(transformPosition, Vector3.forward + Vector3.left * .5f,
            Time.deltaTime * (_sunSpeed + extraSpeed));
        sun.transform.LookAt(transformPosition);
    }

    private void SetSunToDawn()
    {
        sun.transform.position = sunDawnPosition.position;
        sun.transform.rotation = sunDawnPosition.rotation;
    }
}