using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InteractorSelector : MonoBehaviour
{
    private SelectInteractorButton[] _buttons;

    void Start()
    {
        _buttons = GetComponentsInChildren<SelectInteractorButton>();
    }
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            TurnOffAll();
            TurnOnInteractor(InteractorHolder.BlockInteractors.Dig);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            TurnOffAll();
            TurnOnInteractor(InteractorHolder.BlockInteractors.RaiseLand);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            TurnOffAll();
            TurnOnInteractor(InteractorHolder.BlockInteractors.PlaceGreens);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            TurnOffAll();
            TurnOnInteractor(InteractorHolder.BlockInteractors.RaiseWater);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            TurnOffAll();
            TurnOnInteractor(InteractorHolder.BlockInteractors.SendMeteor);
        }
    }

    private void TurnOffAll()
    {
        var iconManagers = GameObject.FindObjectsOfType<IconManager>();
        foreach (var iconManager in iconManagers)
        {
            iconManager.TurnOff();
        }
    }

    private void TurnOnInteractor(InteractorHolder.BlockInteractors interactorType)
    {
        var interactorButton = _buttons.First(b => b.interactorType == interactorType);
        interactorButton.OnClick();
        interactorButton.GetComponent<IconManager>().TurnOn();
    }
}
