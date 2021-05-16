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

    public enum BlockInteractors
    {
        Dig,
        RaiseLand,
        RaiseWater,
        PlaceGreens,
        SendMeteor,
        MakeDesert,
        ConstructHouse,
        ConstructDocks
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

        _interactorComponents.AddRange(new List<BlockInteractor>
            {
                _digInteractor,
                _raiseLandInteractor,
                _raiseWaterInteractor,
                _placeGreensInteractor,
                _sendMeteorInteractor,
                _makeDesertInteractor,
                _constructHouseInteractor,
                _constructDocksInteractor
            }
        );
    }

    public BlockInteractor GetInteractor()
    {
        return _interactorComponents.Find(i => i.enabled);
    }

    public void SetInteractor(BlockInteractors blockInteractor)
    {
        if (blockInteractor == BlockInteractors.Dig)
        {
            _interactorComponents.ForEach(i => i.enabled = false);
            _digInteractor.enabled = true;
        }
        else if (blockInteractor == BlockInteractors.RaiseWater)
        {
            _interactorComponents.ForEach(i => i.enabled = false);
            _raiseWaterInteractor.enabled = true;
        }
        else if (blockInteractor == BlockInteractors.RaiseLand)
        {
            _interactorComponents.ForEach(i => i.enabled = false);
            _raiseLandInteractor.enabled = true;
        }
        else if (blockInteractor == BlockInteractors.PlaceGreens)
        {
            _interactorComponents.ForEach(i => i.enabled = false);
            _placeGreensInteractor.enabled = true;
        }
        else if (blockInteractor == BlockInteractors.SendMeteor)
        {
            _interactorComponents.ForEach(i => i.enabled = false);
            _sendMeteorInteractor.enabled = true;
        }
        else if (blockInteractor == BlockInteractors.MakeDesert)
        {
            _interactorComponents.ForEach(i => i.enabled = false);
            _makeDesertInteractor.enabled = true;
        }
        else if (blockInteractor == BlockInteractors.ConstructHouse)
        {
            _interactorComponents.ForEach(i => i.enabled = false);
            _constructHouseInteractor.enabled = true;
        }
        else if (blockInteractor == BlockInteractors.ConstructDocks)
        {
            _interactorComponents.ForEach(i => i.enabled = false);
            _constructDocksInteractor.enabled = true;
        }
    }
}