using System;
using System.Collections;
using UnityEngine;
using UnityEngine.PlayerLoop;
using Random = UnityEngine.Random;

public class Block : MonoBehaviour
{
    public static float LowestLevel = -3.0f;
    public static float GroundLevel = 0f;

    public enum BlockType
    {
        Grass,
        Water,
        Sand,
        Soil,
        OutsideWater
    }

    public BlockType blockType = BlockType.Grass;

    private bool _frozen;
    private Vector3 _gridPosition;
    private GameObject _occupiedBy;
    private bool _permaFrozen;
    private bool _stable;

    private const int CloudLevel = 4;

    public event Action BeforeDestroy;

    public void DestroySelf()
    {
        OnBeforeDestroy();

        if (_occupiedBy != null)
        {
            DestroyOccupant();
        }

        var root = BlockRoot();
        if (root != null)
        {
            Destroy(root);
        }
    }

    public GameObject BlockRoot()
    {
        try
        {
            if (gameObject == null) return null;
            if (transform.parent == null) return null;

            return transform.parent.gameObject;
        }
        catch
        {
            return null;
        }
    }

    public bool IsVacant()
    {
        return _occupiedBy == null;
    }

    public bool IsLot()
    {
        return blockType == BlockType.Grass;
    }

    public bool IsLand()
    {
        return blockType == BlockType.Grass
               || blockType == BlockType.Soil
               || blockType == BlockType.Sand;
    }

    public bool IsLevelWith(Block block)
    {
        var thisLevelHeight = IsWater() ? _gridPosition.y + 1 : _gridPosition.y;
        var otherLevelHeight = block.IsWater() ? block.GetGridPosition().y + 1 : block.GetGridPosition().y;

        return Math.Abs(otherLevelHeight - thisLevelHeight) < .5f;
    }

    public void ShortFreeze()
    {
        if (_permaFrozen) return;

        _frozen = true;
        StartCoroutine(UnfreezeSoon());

        IEnumerator UnfreezeSoon()
        {
            yield return new WaitForSeconds(.5f);
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

    public void SetGridPosition(Vector3 blockPosition)
    {
        _gridPosition = blockPosition;
    }

    public Vector3 GetGridPosition()
    {
        return _gridPosition;
    }

    public void CreateAndOccupy(GameObject template)
    {
        var occupant = Instantiate(template, null, true);
        occupant.transform.position = Vector3.zero;

        _occupiedBy = occupant;

        var blockRelative = occupant.GetComponent<BlockRelative>();
        if (blockRelative)
        {
            blockRelative.block = this;
        }

        var animationHeight = .4f;
        occupant.transform.position = transform.position + Vector3.up * (.05f + animationHeight);
    }

    public void Occupy(GameObject occupant)
    {
        _occupiedBy = occupant;

        var blockRelative = occupant.GetComponent<BlockRelative>();
        if (blockRelative)
        {
            blockRelative.block = this;
        }

        var animationHeight = .4f;
        occupant.transform.position = transform.position + Vector3.up * (.05f + animationHeight);
    }

    public void SetOccupantThatIsTailFromOtherBase(GameObject occupant)
    {
        _occupiedBy = occupant;

        var blockRelative = occupant.GetComponent<BlockRelative>();
        if (blockRelative)
        {
            blockRelative.block = this;
        }
    }

    public void PlaceOnTopOfSelf(Block otherBlock, GameObject occupantRoot)
    {
        if (!IsVacant())
        {
            throw new Exception("Trying to place something on top of occupied block!");
        }

        _occupiedBy = occupantRoot;

        otherBlock.transform.position = transform.position + new Vector3(0, .1f, 0);

        otherBlock.ShortFreeze();
    }

    public void DestroyOccupant()
    {
        Destroy(_occupiedBy);
        _occupiedBy = null;
    }

    public void PermanentFreeze()
    {
        _frozen = true;
        _permaFrozen = true;
    }

    public void UnPermaFreeze()
    {
        _frozen = false;
        _permaFrozen = false;
    }

    public bool IsGroundLevel() // TODO Rename to SeaLevel
    {
        return Math.Abs(_gridPosition.y) <= (GroundLevel + .5f);
    }

    public bool IsLowestLevel()
    {
        return Math.Abs(_gridPosition.y - LowestLevel) < .5f;
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

    public bool OccupiedByAnotherBlock()
    {
        return !IsVacant() && _occupiedBy.GetComponent<Block>() != null;
    }

    public HouseSpawn GetOccupantHouse()
    {
        if (!OccupiedByHouse()) throw new Exception("Trying to get occupant house, but is not occupied by a house!");

        return _occupiedBy.GetComponent<HouseSpawn>();
    }

    public int DistanceToOtherBlock(Block otherBlock)
    {
        var position = GetGridPosition();
        var x0 = position.x - Mathf.Floor(position.z / 2);
        var y0 = position.z;

        var otherPosition = otherBlock.GetGridPosition();
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

    public bool IsWater()
    {
        return blockType == BlockType.Water;
    }

    public bool OccupiedByGreens()
    {
        return _occupiedBy != null && _occupiedBy.GetComponent<GreensSpawn>() != null;
    }

    public bool OccupiedByGrownGreens()
    {
        if (_occupiedBy == null) return false;

        var greens = _occupiedBy.GetComponent<GreensSpawn>();
        if (!greens) return false;

        return greens.IsGrown();
    }

    public GreensSpawn GetOccupantGreens()
    {
        return _occupiedBy.GetComponent<GreensSpawn>();
    }

    public bool IsSand()
    {
        return blockType == BlockType.Sand;
    }

    public GameObject GetOccupant()
    {
        return _occupiedBy;
    }

    private void OnBeforeDestroy()
    {
        BeforeDestroy?.Invoke();
    }

    public bool HasOccupant()
    {
        return _occupiedBy != null;
    }

    public bool AboveCloudLevel()
    {
        return GetGridPosition().y > CloudLevel;
    }

    public bool BelowCloudLevel()
    {
        return GetGridPosition().y <= CloudLevel;
    }

    public bool OccupiedByDesertHouse()
    {
        return !IsVacant() && _occupiedBy.CompareTag("DesertHouse");
    }

    public bool OccupiedByShrine()
    {
        return !IsVacant() && _occupiedBy.GetComponent<ShrineSpawn>() != null;
    }

    public bool IsTopBlockInStack()
    {
        return IsVacant();
    }

    public bool IsOutsideWater()
    {
        return blockType == BlockType.OutsideWater;
    }

    public void SetAsStable()
    {
        _stable = true;
    }

    public void SetAsUnstable()
    {
        _stable = false;
    }

    public bool IsStable()
    {
        return _stable;
    }
}