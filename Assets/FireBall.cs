﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBall : MonoBehaviour
{
    private bool _destroyed;
    public event Action BeforeDestroy;
    public event Action Expand;

    void Update()
    {
        if (transform.localScale.x > 2.2f)
        {
            if (!_destroyed)
            {
                OnBeforeDestroy();
                _destroyed = true;

                Destroy(gameObject, 1);
            }
        }
        else
        {
            transform.localScale += Vector3.one * (1.05f * Time.deltaTime);
            transform.localScale *= 1 + Time.deltaTime * .5f;

            OnExpand();
        }
    }

    private void OnBeforeDestroy()
    {
        BeforeDestroy?.Invoke();
    }

    private void OnExpand()
    {
        Expand?.Invoke();
    }
}