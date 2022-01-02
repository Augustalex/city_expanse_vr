using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GUIScene : MonoBehaviour
{
    private GameObject _camera;
    private static GUIScene _instance;
    private GameObject _guiContent;

    public static GUIScene Get()
    {
        return _instance;
    }

    private void Awake()
    {
        _camera = GetComponentInChildren<Camera>().gameObject;
        _guiContent = GetComponentInChildren<GUISceneContent>().gameObject;
        
        _instance = this;
    }

    public void Show()
    {
        _camera.SetActive(true);
        _guiContent.SetActive(true);
    }

    public void Hide()
    {
        _camera.SetActive(false);
        _guiContent.SetActive(false);
    }
}
