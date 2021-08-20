using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal.Profiling.Memory.Experimental;
using UnityEngine;

public class MenuScene : MonoBehaviour
{
    private GameObject _camera;
    private GameObject _trophiesRoot;
    private Trophies _trophies;
    private bool _shown = false;

    private void Awake()
    {
        _camera = GetComponentInChildren<MenuCamera>().gameObject;
        _trophies = GetComponentInChildren<Trophies>();
        _trophiesRoot = _trophies.gameObject;
    }

    void Start()
    {
        Hide();
    }

    void Update()
    {
        if (_shown)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Hide();
            }
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Show();
            }
        }
    }

    private void Show()
    {
        _camera.SetActive(true);
        _trophiesRoot.SetActive(true);
        _trophies.Refresh();
        
        _shown = true;
    }

    private void Hide()
    {
        _camera.SetActive(false);
        _trophiesRoot.SetActive(false);

        _shown = false;
    }
}