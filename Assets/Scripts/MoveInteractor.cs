﻿using UnityEngine;

public class MoveInteractor : BlockInteractor
{
    public DigBlockInteractor digBlockInteractor;
    public DigWaterBlockInteractor digWaterBlockInteractor;
    public RaiseLandBlockInteractor raiseLandBlockInteractor;
    public RaiseWaterBlockInteractor raiseWaterBlockInteractor;

    private Block.BlockType _activeBlockType = Block.BlockType.Grass;

    private int _count;
    public override InteractorHolder.BlockInteractors InteractorType => InteractorHolder.BlockInteractors.Move;

    private const int MoveBufferCount = 12;
    private const int DigMouseButton = 1;
    private const int RaiseMouseButton = 0;

    public override bool LockOnLayer()
    {
        return true;
    }

    public override void Interact(GameObject other)
    {
        var blockComponent = other.GetComponent<Block>();

        if (Input.GetMouseButton(DigMouseButton))
        {
            var isWater = blockComponent.IsWater();
            if (isWater)
            {
                digWaterBlockInteractor.Interact(other);
            }
            else
            {
                digBlockInteractor.Interact(other);
                BlockCapacity.Get().IncreaseCount();
            }
        }
        else if (Input.GetMouseButton(RaiseMouseButton))
        {
            if (BlockCapacity.Get().LastBlock())
            {
                raiseLandBlockInteractor.InteractWithAlternateSound(other);
            }
            else
            {
                raiseLandBlockInteractor.Interact(other);
            }

            BlockCapacity.Get().DecreaseCount();
        }
    }

    public void _Interact(GameObject other)
    {
        var blockComponent = other.GetComponent<Block>();

        if (Input.GetMouseButton(DigMouseButton) && _count < MoveBufferCount)
        {
            var isWater = blockComponent.IsWater();
            if (_count == 0)
            {
                _activeBlockType = isWater ? Block.BlockType.Lake : Block.BlockType.Grass;
            }

            _count += 1;

            if (_activeBlockType == Block.BlockType.Lake)
            {
                digWaterBlockInteractor.Interact(other);
            }
            else
            {
                digBlockInteractor.Interact(other);
            }
        }
        else if (Input.GetMouseButton(RaiseMouseButton) && _count > 0)
        {
            if (_count > 0 && _activeBlockType == Block.BlockType.Lake && blockComponent.IsOutsideWater())
            {
                _count = 0;
                PlaySound(BlockSoundLibrary.BlockSound.Water, other.transform.position);
            }
            else
            {
                _count -= 1;

                if (_activeBlockType == Block.BlockType.Lake)
                {
                    raiseWaterBlockInteractor.Interact(other);
                }
                else
                {
                    raiseLandBlockInteractor.Interact(other);
                }
            }
        }
    }

    public override bool Interactable(GameObject other)
    {
        var blockComponent = other.GetComponent<Block>();
        if (!blockComponent) return false;

        if (Input.GetMouseButton(DigMouseButton))
        {
            var isWater = blockComponent.IsWater();
            if (isWater)
            {
                return digWaterBlockInteractor.Interactable(other);
            }
            else if (BlockCapacity.Get().HasMoreCapacity())
            {
                return digBlockInteractor.Interactable(other);
            }
            else
            {
                return false;
            }
        }
        else if (Input.GetMouseButton(RaiseMouseButton) && BlockCapacity.Get().HasBlocks())
        {
            return raiseLandBlockInteractor.Interactable(other);
        }
        else
        {
            return false;
        }
    }

    public bool _Interactable(GameObject other)
    {
        var blockComponent = other.GetComponent<Block>();
        if (!blockComponent) return false;

        if (Input.GetMouseButton(DigMouseButton))
        {
            var isWater = blockComponent.IsWater();
            if (_count == 0)
            {
                _activeBlockType = isWater ? Block.BlockType.Lake : Block.BlockType.Grass;
            }

            if (_activeBlockType == Block.BlockType.Lake)
            {
                return digWaterBlockInteractor.Interactable(other);
            }
            else
            {
                return digBlockInteractor.Interactable(other);
            }
        }
        else if (Input.GetMouseButton(RaiseMouseButton) && _count > 0)
        {
            if (_count > 0 && _activeBlockType == Block.BlockType.Lake && blockComponent.IsOutsideWater()) return true;

            if (_activeBlockType == Block.BlockType.Lake)
            {
                return raiseWaterBlockInteractor.Interactable(other);
            }
            else
            {
                return raiseLandBlockInteractor.Interactable(other);
            }
        }
        else
        {
            return false;
        }
    }
}