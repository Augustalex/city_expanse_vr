using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BonfireScene : MonoBehaviour
{
    private GameObject _camera;
    private static BonfireScene _instance;
    private DevoteeDisplay _devoteeDisplay;
    private BonfireStateManager _bonfireState;

    public static BonfireScene Get()
    {
        return _instance;
    }

    private void Awake()
    {
        _instance = this;
        
        _camera = GetComponentInChildren<Camera>().gameObject;
        _devoteeDisplay = GetComponentInChildren<DevoteeDisplay>();
    }

    void Start()
    {
        _bonfireState = BonfireStateManager.Get();
    }

    public void Show()
    {
        _devoteeDisplay.SetCount(_bonfireState.GetDevoteeCount());

        _camera.SetActive(true);
    }

    public void Hide()
    {
        StartCoroutine(DoSoon());

        IEnumerator DoSoon()
        {
            yield return new WaitForEndOfFrame();

            _camera.SetActive(false);
        }
    }
}