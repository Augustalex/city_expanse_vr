using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceGreensBlockInteractor : BlockInteractor
{
    public GameObject greensBlockTemplate;
    public GameObject desertGreensTemplate;

    public void StopRigidbodySoon(GameObject greens)
    {
        StartCoroutine(DoSoon());

        IEnumerator DoSoon()
        {
            yield return new WaitForSeconds(2);

            greens.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
        }
    }

    public override bool Interactable(GameObject other)
    {
        if (!IsActivated()) return false;

        var blockComponent = other.gameObject.GetComponent<Block>();
        return blockComponent
               && blockComponent.IsInteractable()
               && blockComponent.IsVacant()
               && blockComponent.BelowCloudLevel();
    }

    public override bool LockOnLayer()
    {
        return false;
    }

    public override void Interact(GameObject other)
    {
        var blockComponent = other.gameObject.GetComponent<Block>();

        if (blockComponent.IsSand())
        {
            InhabitBlock(blockComponent, desertGreensTemplate);
        }
        else if (blockComponent.IsGrass())
        {
            InhabitBlock(blockComponent, greensBlockTemplate);
        }
    }

    private void InhabitBlock(Block blockComponent, GameObject greensTemplate)
    {
        var greens = Instantiate(greensTemplate);
        blockComponent.Occupy(greens);
        blockComponent.ShortFreeze();
        PlaySound(BlockSoundLibrary.BlockSound.PlaceItem);

        StopRigidbodySoon(greens);
    }
}