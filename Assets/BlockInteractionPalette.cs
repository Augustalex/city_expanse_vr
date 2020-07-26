using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockInteractionPalette : MonoBehaviour
{
    private BlockInteractor _activeInteractor;

    public void Select(BlockInteractor interactor)
    {
        var previousInteractor = _activeInteractor;
        if (previousInteractor)
        {
            _activeInteractor = null;
            previousInteractor.Deactivate();
            previousInteractor.transform.position = interactor.transform.position;
        }

        interactor.Activate();
        _activeInteractor = interactor;
    }
}
