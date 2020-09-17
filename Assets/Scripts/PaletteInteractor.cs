using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaletteInteractor : MonoBehaviour
{
    private BlockInteractor _blockInteractor;

    void Start()
    {
        _blockInteractor = GetComponent<BlockInteractor>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!_blockInteractor.IsActivated()) return;

        
        var interactorComponent = other.gameObject.GetComponent<BlockInteractor>();
        
        // if (interactorComponent.GetType() == typeof(DigBlockInteractor))
        // {
        //     Debug.Log("DIG!");
        // }
        if (interactorComponent && !interactorComponent.IsActivated() && interactorComponent.IsInteractable())
        {
            var palette = GetComponentInParent<BlockInteractionPalette>();
            palette.Select(interactorComponent);
            _blockInteractor.PlayGeneralSound();
        }
    }
}
