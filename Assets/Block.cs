using System.Collections;
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
    private Vector2 _position;
    private GameObject _occupiedBy;

    public void DestroySelf()
    {
        GetWorldPlane().RemoveBlockAt(_position);

        Destroy(gameObject);
        Destroy(_occupiedBy);
    }

    public void TurnOverSpotTo(GameObject otherBlock)
    {
        var worldPlane = GetWorldPlane();
        worldPlane.RemoveBlockAt(_position);
        worldPlane.AddBlockToPosition(otherBlock, _position);

        otherBlock.transform.position = transform.position;

        DestroySelf();
    }

    public bool Vacant()
    {
        return _occupiedBy == null;
    }

    public void ShortFreeze()
    {
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
        return !_frozen;
    }

    public WorldPlane GetWorldPlane()
    {
        return GameObject.FindWithTag("WorldPlane").GetComponent<WorldPlane>();
    }

    public void SetPosition(Vector2 blockPosition)
    {
        _position = blockPosition;
    }

    public Vector2 GetPosition()
    {
        return _position;
    }

    public void Occupy(GameObject house)
    {
        _occupiedBy = house;

        var animationHeight = .05f;
        house.transform.position = transform.position + Vector3.up * (.05f + animationHeight);
    }
}