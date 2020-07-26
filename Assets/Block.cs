using System;
using System.Collections;
using System.Linq.Expressions;
using UnityEngine;

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
        worldPlane.AddBlockToPosition(otherBlock, _position); // TODO Is there someway to remove this circular dependency perhaps. It is very confusing!
        
        DestroySelf();
    }

    public bool Vacant()
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
}