using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingScene : MonoBehaviour
{
    private GameObject _loadingCloak;
    private GameObject _loadingCamera;

    // Start is called before the first frame update
    void Start()
    {
        _loadingCamera = GetComponentInChildren<Camera>().gameObject;
        _loadingCloak = GetComponentInChildren<LoadingCloak>().gameObject;
    }

    public void TurnOff()
    {
        _loadingCloak.SetActive(false);
        _loadingCamera.SetActive(false);
    }
}
