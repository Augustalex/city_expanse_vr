using System;
using UnityEngine;

public class IconClicker : MonoBehaviour
{
    private Camera _mainCamera;
    private InteractorHolder _interactorHolder;

    private void Start()
    {
        _interactorHolder = GetComponent<InteractorHolder>();
        _mainCamera = GetComponent<Camera>();
    }

    private void Update()
    {
        if (GetPointerSubmit())
        {
            RaycastHit hit;
            Ray ray = _mainCamera.ScreenPointToRay(GetPointerPosition());
            if (!Physics.Raycast(ray, out hit, 1000.0f)) return;

            if (hit.collider.CompareTag("IconButton"))
            {
                hit.collider.GetComponent<SelectInteractorButton>().OnClick();

                var iconManagers = GameObject.FindObjectsOfType<IconManager>();
                foreach (var iconManager in iconManagers)
                {
                    iconManager.TurnOff();
                }
                
                hit.collider.GetComponent<IconManager>().TurnOn();
            }
            else if (hit.collider.CompareTag("SunClock"))
            {
                var iconManagers = GameObject.FindObjectsOfType<IconManager>();
                foreach (var iconManager in iconManagers)
                {
                    iconManager.TurnOff();
                }
                
                _interactorHolder.CancelCurrentInteractor();
            }
        }
    }
    
    private Vector2 GetPointerPosition()
    {
        return Input.touchCount > 0 ? Input.GetTouch(0).position : (Vector2) Input.mousePosition;
    }

    private bool GetPointerSubmit()
    {
        return Input.GetMouseButtonDown(0) || (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Began);
    }
}