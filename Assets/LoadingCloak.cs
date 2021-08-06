﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingCloak : MonoBehaviour
{
    void Start()
    {
        FlatInterfaceController.Get().Disable();
        StartCoroutine(DoSoon());
        
        IEnumerator DoSoon()
        {
            yield return new WaitForSeconds(3);
            Destroy(gameObject);
            FlatInterfaceController.Get().Enable();
        }
    }
}
