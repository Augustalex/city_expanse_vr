using System;
using System.Collections;
using System.Linq.Expressions;
using UnityEngine;
using Random = UnityEngine.Random;

public class Block : MonoBehaviour
{
    public enum BlockType
    {
        Grass,
        Water
    }

    public BlockType blockType = BlockType.Grass;

    private bool _frozen;
    private Vector3 _position;
    private GameObject _occupiedBy;
    private bool _permaFrozen;

    public void DestroySelf()
    {
        GetWorldPlane().RemoveBlockAt(_position);

        Destroy(gameObject);
        Destroy(_occupiedBy);
    }

    public void TurnOverSpotTo(Block otherBlock)
    {
        var worldPlane = GetWorldPlane();
        worldPlane.RemoveBlockAt(_position);
        worldPlane.AddBlockToPosition(otherBlock,
            _position); // TODO Is there someway to remove this circular dependency perhaps. It is very confusing!

        DestroySelf();
    }

    public bool IsVacant()
    {
        return _occupiedBy == null;
    }

    public void ShortFreeze()
    {
        if (_permaFrozen) return;

        _frozen = true;
        StartCoroutine(UnfreezeSoon());

        IEnumerator UnfreezeSoon()
        {
            yield return new WaitForSeconds(1);
            _frozen = false;
        }
    }

    public bool IsInteractable()
    {
        return !_frozen && !IsPermaFrozen();
    }

    public bool IsPermaFrozen()
    {
        return _permaFrozen;
    }

    public WorldPlane GetWorldPlane()
    {
        return GameObject.FindWithTag("WorldPlane").GetComponent<WorldPlane>();
    }

    public void SetPosition(Vector3 blockPosition)
    {
        _position = blockPosition;
        transform.parent.position = RealPosition();
    }

    public Vector3 GetPosition()
    {
        return _position;
    }

    private Vector3 RealPosition()
    {
        return GetWorldPlane().ToRealCoordinates(_position);
    }

    public void Occupy(GameObject house)
    {
        _occupiedBy = house;

        var animationHeight = .05f;
        house.transform.position = transform.position + Vector3.up * (.05f + animationHeight);
    }

    public void PlaceOnTopOfSelf(Block otherBlock, GameObject occupantRoot)
    {
        _occupiedBy = occupantRoot;

        GetWorldPlane().AddBlockToPosition(otherBlock, _position + Vector3.up);
        otherBlock.transform.position = transform.position + new Vector3(0, .1f, 0);

        otherBlock.ShortFreeze();
    }

    public void PermanentFreeze()
    {
        _frozen = true;
        _permaFrozen = true;
    }

    public bool IsGroundLevel()
    {
        return Math.Abs(_position.y) < .5f;
    }

    public void RandomRotateAlongY()
    {
        var blockRoot = transform.parent;
        
        var transformRotation = blockRoot.rotation;
        blockRoot.rotation = Quaternion.Euler(transformRotation.x, (Random.Range(0, 5) * 60), transformRotation.z);
    }

    public bool OccupiedByHouse()
    {
        return !IsVacant() && _occupiedBy.CompareTag("HouseSpawn");
    }

    public HouseSpawn GetOccupantHouse()
    {
        if(!OccupiedByHouse()) throw new Exception("Trying to get occupant house, but is not occupied by a house!");

        return _occupiedBy.GetComponent<HouseSpawn>();
    }

    public int DistanceToOtherBlock(Block otherBlock)
    {
        var position = GetPosition();
        var x0 = position.x - Mathf.Floor(position.z / 2);
        var y0 = position.z;
        
        var otherPosition = otherBlock.GetPosition();
        var x1 = otherPosition.x - Mathf.Floor(otherPosition.z / 2);
        var y1 = otherPosition.z;
        var dx = x1 - x0;
        var dy = y1 - y0;
        var dist = Mathf.Max(Mathf.Abs(dx), Mathf.Abs(dy), Mathf.Abs(dx + dy));
        return Convert.ToInt32(Mathf.Round(dist));
    }

    public bool IsGrass()
    {
        return blockType == BlockType.Grass;
    }
}