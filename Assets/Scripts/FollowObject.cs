using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowObject : MonoBehaviour
{
    public CameraRigContainer.FingerType fingerType;

    private GameObject _target;

    private void Start()
    {
        _target = CameraRigContainer.Get().GetFingerFromType(fingerType);
    }

    void Update()
    {
        transform.position = _target.transform.position;
    }
}