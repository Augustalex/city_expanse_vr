using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BlockInteractionPalette : MonoBehaviour
{
    private BlockInteractor _activeInteractor;
    private GameObject _itemContainer;
    private List<GameObject> _interactors;
    
    public GameObject homeSphere;

    void Start()
    {
        _itemContainer = GetComponentInChildren<ItemContainer>().gameObject;
        _interactors = GetComponentsInChildren<BlockInteractor>().Select(interactor => interactor.gameObject).ToList();

        StartCoroutine(HideSoon());
        
        IEnumerator HideSoon()
        {
            yield return new WaitForSeconds(1);
            
            Hide();
        }
    }

    public void Select(BlockInteractor interactor)
    {
        var previousInteractor = _activeInteractor;
        if (previousInteractor)
        {
            _activeInteractor = null;
            previousInteractor.DeactivateFromPalette();
            previousInteractor.ResetPosition();
        }

        interactor.Activate();
        _activeInteractor = interactor;
    }

    public void Show()
    {
        foreach (var interactor in _interactors)
        {
            interactor.SetActive(true);
        }

        _itemContainer.SetActive(true);
        homeSphere.SetActive(true);
    }

    public void Hide()
    {
        foreach(var interactor in _interactors)
        {
            if (!interactor.GetComponent<BlockInteractor>().IsActivated())
            {
                interactor.SetActive(false);
            }
        }
        
        _itemContainer.SetActive(false);
        
        homeSphere.SetActive(false);
    }
}
