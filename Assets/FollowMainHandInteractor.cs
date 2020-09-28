using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowMainHandInteractor : MonoBehaviour
{
    private Transform _followPoint;

    private void Start()
    {
        _followPoint = HandInteractorFollowPoint.Get().followPoint;
    }

    void FixedUpdate()
    {
        transform.position = _followPoint.position;
    }
}