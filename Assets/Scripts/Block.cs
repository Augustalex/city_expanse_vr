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
        Lake,
        Sand,
        Soil,
        OutsideWater,
        OceanWater
    }

    public enum HighlightType
    {
        Interactable,
        NonInteractable
    }

    public BlockType blockType = BlockType.Grass;

    private bool _frozen;
    public Vector3 _gridPosition;
    private GameObject _occupiedBy;
    private bool _isOccupied;
    private bool _permaFrozen;
    private bool _stable;
    private Block _blockBeneath = null;
    private OccupyingType _occupantType = OccupyingType.Null;
    private Block _blockAbove = null;
    private bool _waterLocked;
    private Vector3 _originalScale;
    private bool _highlighted;
    private float _highlightedAt;
    private string _highlightedBy;

    private const int CloudLevel = 4;

    public event Action BeforeDestroy;

    void Start()
    {
        _originalScale = transform.localScale;
    }

    public void DestroySelf()
    {
        OnBeforeDestroy();

        if (_occupiedBy != null)
        {
            DestroyOccupant();
        }

        if (_blockBeneath != null && gameObject.transform.parent != null)
        {
            _blockBeneath.RemoveAsOccupant(gameObject.transform.parent.gameObject);
        }

        DestroyBlockRootOrSelf();
    }

    public void RemoveAsOccupant(GameObject selfAsProclaimedOccupant)
    {
        if (selfAsProclaimedOccupant == _occupiedBy)
        {
            ResetOccupantInfo();
            MakeSureTopGrassBlocksHaveCorrectTexture();
        }
    }

    public void DestroyBlockRootOrSelf()
    {
        try
        {
            if (gameObject == null && transform.parent == null)
            {
                Destroy(gameObject);
            }
            else
            {
                Destroy(transform.parent.gameObject);
            }
        }
        catch
        {
            // ignored
        }
    }

    public bool IsVacant()
    {
        return !_isOccupied;
    }

    public bool OccupiedByBlock()
    {
        return GetOccupantType() == OccupyingType.Block;
    }


    public bool IsLot()
    {
        if (FeatureToggles.Get().desertsAreBeaches)
        {
            return IsGrass() || IsSand();
        }
        else
        {
            return IsGrass();
        }
    }

    public bool IsLand()
    {
        return blockType == BlockType.Grass
               || blockType == BlockType.Soil
               || blockType == BlockType.Sand;
    }

    public bool
        IsLevelWith(
            Block block) // This seems to do something strange with water. Perhaps it is this logic that makes water fall as waterfalls, instead of filling up everything as a plane?
    {
        var thisLevelHeight = IsWater() ? _gridPosition.y + 1 : _gridPosition.y;
        var otherLevelHeight = block.IsWater() ? block.GetGridPosition().y + 1 : block.GetGridPosition().y;

        return Math.Abs(otherLevelHeight - thisLevelHeight) < .5f;
    }

    public bool IsOnSameHeightAs(Block block)
    {
        return Math.Abs(block.GetGridPosition().y - _gridPosition.y) < .5f;
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

        SetOccupiedBy(occupant);

        var blockRelative = occupant.GetComponent<BlockRelative>();
        if (blockRelative)
        {
            blockRelative.block = this;
        }

        var animationHeight = .4f;
        occupant.transform.position = transform.position + Vector3.up * (.05f + animationHeight);
    }

    private void SetOccupiedBy(GameObject occupant)
    {
        _occupiedBy = occupant;

        var blockOfOccupant = occupant.GetComponentInChildren<Block>();
        if (blockOfOccupant != null)
        {
            blockOfOccupant.SetBlockBeneath(this);
            _occupantType = OccupyingType.Block;
            _blockAbove = blockOfOccupant;
        }
        else
        {
            _occupantType = GetOccupantType(occupant);
        }

        _isOccupied = true;
    }

    private void SetBlockBeneath(Block blockBeneath)
    {
        _blockBeneath = blockBeneath;
    }

    public void Occupy(GameObject occupant)
    {
        SetOccupiedBy(occupant);

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
        SetOccupiedBy(occupant);

        var blockRelative = occupant.GetComponent<BlockRelative>();
        if (blockRelative)
        {
            blockRelative.block = this;
        }
    }

    public void PlaceOnTop(Block otherBlock, GameObject occupantRoot)
    {
        if (!IsVacant())
        {
            throw new Exception("Trying to place something on top of occupied block!");
        }

        SetOccupiedBy(occupantRoot);
        otherBlock.transform.position = transform.position + new Vector3(0, .1f, 0);

        otherBlock.ShortFreeze();
    }

    public void DestroyOccupant()
    {
        if (_occupiedBy != null)
        {
            var occupantBlock = _occupiedBy.GetComponent<Block>();
            if (occupantBlock != null)
            {
                occupantBlock.DestroySelf();
            }
            else
            {
                Destroy(_occupiedBy);
            }
        }

        ResetOccupantInfo();
    }

    private OccupyingType GetOccupantType()
    {
        return _occupantType;
    }

    private void ResetOccupantInfo()
    {
        _occupiedBy = null;
        _isOccupied = false;
        _occupantType = OccupyingType.Null;
        _blockAbove = null;
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
        return blockType == BlockType.Lake || blockType == BlockType.OceanWater;
    }

    enum OccupyingType
    {
        House,
        Greens,
        DesertHouse,
        Shrine,
        Block,
        Misc,
        Docks,
        Null
    }

    private OccupyingType GetOccupantType(GameObject occupant)
    {
        var house = occupant.CompareTag("HouseSpawn");
        if (house) return OccupyingType.House;

        var greens = occupant.GetComponent<GreensSpawn>();
        if (greens) return OccupyingType.Greens;

        var desert = occupant.CompareTag("DesertHouse");
        if (desert) return OccupyingType.DesertHouse;

        var shrine = occupant.GetComponent<ShrineSpawn>();
        if (shrine) return OccupyingType.Shrine;

        var docks = occupant.GetComponent<DocksSpawn>();
        if (docks) return OccupyingType.Docks;

        return OccupyingType.Misc;
    }

    public bool OccupiedByHouse()
    {
        return !IsVacant() && _occupantType == OccupyingType.House;
    }


    public bool OccupiedByGreens()
    {
        return HasOccupant() && _occupantType == OccupyingType.Greens;
    }

    public bool OccupiedByGrownGreens()
    {
        if (IsVacant()) return false;

        return _occupantType == OccupyingType.Greens
               && _occupiedBy.GetComponent<GreensSpawn>().IsGrown();
    }

    public bool OccupiedByDesertHouse()
    {
        return !IsVacant() && _occupantType == OccupyingType.DesertHouse;
    }

    public bool OccupiedByShrine()
    {
        return !IsVacant() && _occupantType == OccupyingType.Shrine;
    }

    public bool OccupiedByDocks()
    {
        return !IsVacant() && _occupantType == OccupyingType.Docks;
    }

    public GreensSpawn GetOccupantGreens()
    {
        return _occupiedBy.GetComponent<GreensSpawn>();
    }

    public Block GetTopBlock()
    {
        var current = this;
        while (current.OccupiedByBlock())
        {
            current = current._blockAbove;
        }

        return current;
    }

    public void MakeSureTopGrassBlocksHaveCorrectTexture()
    {
        var current = this;
        while (current._blockBeneath != null)
        {
            current = current._blockBeneath;
        }

        while (current._blockAbove)
        {
            current.GetComponent<GrassBlock>().SetNormalMaterial();
            current = current._blockAbove;
        }

        if (current._gridPosition.y > Block.CloudLevel)
        {
            current.GetComponent<GrassBlock>().SetNormalMaterial();
        }
        else
        {
            current.GetComponent<GrassBlock>().SetTopMaterial();
        }
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
        return !IsVacant();
    }

    public bool AboveCloudLevel()
    {
        return GetGridPosition().y > CloudLevel;
    }

    public bool BelowCloudLevel()
    {
        return GetGridPosition().y <= CloudLevel;
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

    public void SetWaterLocked(bool isWaterLocked)
    {
        _waterLocked = isWaterLocked;
    }

    public bool IsWaterLocked()
    {
        return _waterLocked;
    }

    public bool CanBeDugAsAnIndependentBlock()
    {
        if (FeatureToggles.Get().desertsAreBeaches)
        {
            return IsGrass() || IsSand();
        }
        else
        {
            return IsGrass();
        }
    }

    public bool IsLake()
    {
        return blockType == BlockType.Lake;
    }

    public void Highlight(string highlighterIdentifier, HighlightType highlightType)
    {
        _highlighted = true;
        _highlightedBy = highlighterIdentifier;
        _highlightedAt = Time.fixedTime;

        if (highlightType == HighlightType.Interactable)
        {
            transform.localScale = _originalScale * 1.1f;
        }
        else if (highlightType == HighlightType.NonInteractable)
        {
        }
    }

    public bool ShouldRemoveHighlight(string highlighterIdentifier)
    {
        return _highlighted
               && _highlightedBy == highlighterIdentifier
               && Time.fixedTime - _highlightedAt > 3;
    }

    public void RemoveHighlight()
    {
        transform.localScale = _originalScale;
        _highlighted = false;
    }
}