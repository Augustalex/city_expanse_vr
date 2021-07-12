using System;
using UnityEngine;

public class IconClicker : MonoBehaviour
{
    private Camera _mainCamera;

    private void Start()
    {
        _mainCamera = GetComponent<Camera>();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
            if (!Physics.Raycast(ray, out hit, 1000.0f)) return;

            if (hit.collider.CompareTag("IconButton"))
            {
                hit.collider.GetComponent<SelectInteractorButton>().OnClick();
            }
        }
    }
}