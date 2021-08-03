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
        if (transform.localScale.x > 4.2f)
        {
            if (!_destroyed)
            {
                OnBeforeDestroy();
                _destroyed = true;

                Destroy(gameObject, 2);
            }
        }
        else
        {
            transform.localScale += new Vector3(1, .2f, 1) * (.5f * Time.deltaTime);
            transform.localScale *= 1 + Time.deltaTime * .1f;

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