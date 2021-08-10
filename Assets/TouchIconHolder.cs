using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchIconHolder : MonoBehaviour
{
    void Start()
    {
        if (!TouchGlobals.usingTouch)
        {
            gameObject.SetActive(false);
        }
    }
}
