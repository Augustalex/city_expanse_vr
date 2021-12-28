using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingCloak : MonoBehaviour
{
    private WorldPlane _worldPlane;
    private static readonly int Out = Animator.StringToHash("FadeOut");

    void Start()
    {
        FlatInterfaceController.Get().Disable();

        _worldPlane = WorldPlane.Get();
    }

    private void Update()
    {
        if (_worldPlane.WorldGenerationDone())
        {
            FadeOut();
            FlatInterfaceController.Get().Enable();
        }
    }

    private void FadeOut()
    {
        GetComponent<Animator>().SetTrigger(Out);
    }

    public void DestroySelf()
    {
        GetComponentInParent<LoadingScene>().TurnOff();
    }
}
