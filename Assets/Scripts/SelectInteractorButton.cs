using UnityEngine;
using UnityEngine.UI;

public class SelectInteractorButton : MonoBehaviour
{
    public InteractorHolder interactorHolder;
    public InteractorHolder.BlockInteractors interactorType;
    private WorkQueue _workQueue;
    private int _ticket;
    private bool _enabled;

    void Start()
    {
        _workQueue = WorkQueue.Get();
    }

    public void OnClick()
    {
        interactorHolder.SetInteractor(interactorType);
    }

    public void Update()
    {
        if (CanUpdateStatusThisFrame())
        {
            _enabled = Enabled();

            if (!_enabled)
            {
                if (interactorHolder.HoldingInteractorOfType(interactorType))
                {
                    interactorHolder.CancelCurrentInteractor();
                }
            }
        }

        GetComponent<Button>().interactable = _enabled;
    }

    private bool CanUpdateStatusThisFrame()
    {
        if (_workQueue.HasExpiredTicket(_ticket))
        {
            _ticket = _workQueue.GetTicket();
        }

        return _workQueue.HasTicketForFrame(_ticket);
    }

    public bool Enabled()
    {
        if (interactorType == InteractorHolder.BlockInteractors.ConstructHouse)
        {
            return CanConstructHouse();
        }
        else if (interactorType == InteractorHolder.BlockInteractors.ConstructDocks)
        {
            return CanConstructDocks();
        }
        else if (interactorType == InteractorHolder.BlockInteractors.ConstructFarm)
        {
            return CanConstructFarms();
        }
        else
        {
            return true;
        }
    }

    private bool CanConstructFarms()
    {
        return CityFarms.Get().CanManuallyConstructAnyKindOfFarm();
    }

    private bool CanConstructDocks()
    {
        return CityDocks.Get().CanBuildADock();
    }

    private bool CanConstructHouse()
    {
        return City.Get().CanSpawnAnotherHouse();
    }
}