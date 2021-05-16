using System;
using System.Collections.Generic;
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
        _raiseLandInteractor.enabled = false;

        _raiseWaterInteractor = GetComponent<RaiseWaterBlockInteractor>();
        _raiseWaterInteractor.enabled = false;

        _placeGreensInteractor = GetComponent<PlaceGreensBlockInteractor>();
        _placeGreensInteractor.enabled = false;

        _sendMeteorInteractor = GetComponent<SendMeteorBlockInteractor>();
        _sendMeteorInteractor.enabled = false;

        _makeDesertInteractor = GetComponent<MakeDesertInteractor>();
        _makeDesertInteractor.enabled = false;

        _constructHouseInteractor = GetComponent<ConstructHouseBlockInteractor>();
        _constructHouseInteractor.enabled = false;

        _constructDocksInteractor = GetComponent<ConstructDocksBlockInteractor>();
        _constructDocksInteractor.enabled = false;

        _constructFarmBlockInteractor = GetComponent<ConstructFarmBlockInteractor>();
        _constructFarmBlockInteractor.enabled = false;

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

    public BlockInteractor GetInteractor()
    {
        return _interactorComponents.Find(i => i.enabled);
    }

    public void SetInteractor(BlockInteractors blockInteractor)
    {
        DeactivateAllInteractors();

        if (blockInteractor == BlockInteractors.Dig)
        {
            _digInteractor.enabled = true;
        }
        else if (blockInteractor == BlockInteractors.RaiseWater)
        {
            _raiseWaterInteractor.enabled = true;
        }
        else if (blockInteractor == BlockInteractors.RaiseLand)
        {
            _raiseLandInteractor.enabled = true;
        }
        else if (blockInteractor == BlockInteractors.PlaceGreens)
        {
            _placeGreensInteractor.enabled = true;
        }
        else if (blockInteractor == BlockInteractors.SendMeteor)
        {
            _sendMeteorInteractor.enabled = true;
        }
        else if (blockInteractor == BlockInteractors.MakeDesert)
        {
            _makeDesertInteractor.enabled = true;
        }
        else if (blockInteractor == BlockInteractors.ConstructHouse)
        {
            _constructHouseInteractor.enabled = true;
        }
        else if (blockInteractor == BlockInteractors.ConstructDocks)
        {
            _constructDocksInteractor.enabled = true;
        }
        else if (blockInteractor == BlockInteractors.ConstructFarm)
        {
            _constructFarmBlockInteractor.enabled = true;
        }
    }

    private void DeactivateAllInteractors()
    {
        foreach (var i in _interactorComponents)
        {
            if (i.IsActivated())
            {
                i.Deactivate();
            }
        }
    }
}