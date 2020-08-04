using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(InteractorHolder))]
public class TileClicker : MonoBehaviour
{
    public Camera mainCamera;
    
    private InteractorHolder _interactorHolder;

    void Start()
    {
        _interactorHolder = GetComponent<InteractorHolder>();
    }

    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            RaycastHit hit;
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, 1000.0f))
            {   
                _interactorHolder.GetInteractor().Interact(hit.collider.gameObject);
            }
        }
    }
}
