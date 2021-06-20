using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(DigBlockInteractor))]
[RequireComponent(typeof(RaiseLandBlockInteractor))]
[RequireComponent(typeof(RaiseWaterBlockInteractor))]
[RequireComponent(typeof(PlaceGreensBlockInteractor))]
public class InteractorHolder : MonoBehaviour
{
    private DigBlockInteractor _digInteractor;
    private List<BlockInteractor> _interactorComponents = new List<BlockInteractor>();
    private RaiseLandBlockInteractor _raiseLandInteractor;
    private RaiseWaterBlockInteractor _raiseWaterInteractor;
    private PlaceGreensBlockInteractor _placeGreensInteractor;
    private SendMeteorBlockInteractor _sendMeteorInteractor;
    private MakeDesertInteractor _makeDesertInteractor;
    private ConstructHouseBlockInteractor _constructHouseInteractor;
    private ConstructDocksBlockInteractor _constructDocksInteractor;
    private ConstructFarmBlockInteractor _constructFarmBlockInteractor;
    private bool _hasInteractor;

    public enum BlockInteractors
    {
        Dig,
        RaiseLand,
        RaiseWater,
        PlaceGreens,
        SendMeteor,
        MakeDesert,
        ConstructHouse,
        ConstructDocks,
        ConstructFarm
    }

    private void Start()
    {
        _digInteractor = GetComponent<DigBlockInteractor>();

        _raiseLandInteractor = GetComponent<RaiseLandBlockInteractor>();
        _raiseLandInteractor.interactorEnabled = false;

        _raiseWaterInteractor = GetComponent<RaiseWaterBlockInteractor>();
        _raiseWaterInteractor.interactorEnabled = false;

        _placeGreensInteractor = GetComponent<PlaceGreensBlockInteractor>();
        _placeGreensInteractor.interactorEnabled = false;

        _sendMeteorInteractor = GetComponent<SendMeteorBlockInteractor>();
        _sendMeteorInteractor.interactorEnabled = false;

        _makeDesertInteractor = GetComponent<MakeDesertInteractor>();
        _makeDesertInteractor.interactorEnabled = false;

        _constructHouseInteractor = GetComponent<ConstructHouseBlockInteractor>();
        _constructHouseInteractor.interactorEnabled = false;

        _constructDocksInteractor = GetComponent<ConstructDocksBlockInteractor>();
        _constructDocksInteractor.interactorEnabled = false;

        _constructFarmBlockInteractor = GetComponent<ConstructFarmBlockInteractor>();
        _constructFarmBlockInteractor.interactorEnabled = false;

        _interactorComponents.AddRange(new List<BlockInteractor>
            {
                _digInteractor,
                _raiseLandInteractor,
                _raiseWaterInteractor,
                _placeGreensInteractor,
                _sendMeteorInteractor,
                _makeDesertInteractor,
                _constructHouseInteractor,
                _constructDocksInteractor,
                _constructFarmBlockInteractor
            }
        );
    }

    public bool AnyInteractorActive()
    {
        return _hasInteractor;
    }

    public BlockInteractor GetInteractor()
    {
        return _interactorComponents.Find(i => i.interactorEnabled);
    }

    public void SetInteractor(BlockInteractors blockInteractor)
    {
        DeactivateAllInteractors();

        if (blockInteractor == BlockInteractors.Dig)
        {
            _digInteractor.interactorEnabled = true;
        }
        else if (blockInteractor == BlockInteractors.RaiseWater)
        {
            _raiseWaterInteractor.interactorEnabled = true;
        }
        else if (blockInteractor == BlockInteractors.RaiseLand)
        {
            _raiseLandInteractor.interactorEnabled = true;
        }
        else if (blockInteractor == BlockInteractors.PlaceGreens)
        {
            _placeGreensInteractor.interactorEnabled = true;
        }
        else if (blockInteractor == BlockInteractors.SendMeteor)
        {
            _sendMeteorInteractor.interactorEnabled = true;
        }
        else if (blockInteractor == BlockInteractors.MakeDesert)
        {
            _makeDesertInteractor.interactorEnabled = true;
        }
        else if (blockInteractor == BlockInteractors.ConstructHouse)
        {
            _constructHouseInteractor.interactorEnabled = true;
        }
        else if (blockInteractor == BlockInteractors.ConstructDocks)
        {
            _constructDocksInteractor.interactorEnabled = true;
        }
        else if (blockInteractor == BlockInteractors.ConstructFarm)
        {
            _constructFarmBlockInteractor.interactorEnabled = true;
        }

        _hasInteractor = true;
    }

    private void DeactivateAllInteractors()
    {
        foreach (var i in _interactorComponents)
        {
            DeactivateInteractor(i);
        }
    }

    private void DeactivateInteractor(BlockInteractor interactor)
    {
        if (interactor.IsActivated())
        {
            interactor.Deactivate();
        }
    }

    public bool HoldingInteractorOfType(BlockInteractors interactorType)
    {
        if (!_hasInteractor) return false;

        return GetInteractor().InteractorType == interactorType;
    }

    public void CancelCurrentInteractor()
    {
        if (_hasInteractor)
        {
            DeactivateInteractor(GetInteractor());
            _hasInteractor = false;
        }
    }
}