using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceGreensBlockInteractor : BlockInteractor
{
    public GameObject greensBlockTemplate;
    public GameObject desertGreensTemplate;
    private bool _hasPlacedSomeGreens;

    private void OnTriggerEnter(Collider other)
    {
        Interact(other.gameObject);
    }

    public void StopRigidbodySoon(GameObject greens)
    {
        StartCoroutine(DoSoon());

        IEnumerator DoSoon()
        {
            yield return new WaitForSeconds(2);

            greens.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
        }
    }

    public override void Interact(GameObject other)
    {
        if (!IsActivated()) return;

        var blockComponent = other.gameObject.GetComponent<Block>();
        if (blockComponent
            
            && blockComponent.IsInteractable()
            && blockComponent.IsVacant()
            && blockComponent.BelowCloudLevel()
            )
        {
            if (blockComponent.IsSand())
            {
                InhabitBlock(blockComponent, desertGreensTemplate);
            }
            else if (blockComponent.IsGrass())
            {
                InhabitBlock(blockComponent, greensBlockTemplate);
            }
        }
    }

    private void InhabitBlock(Block blockComponent, GameObject greensTemplate)
    {
        var greens = Instantiate(greensTemplate);
        blockComponent.Occupy(greens);
        blockComponent.ShortFreeze();
        PlayGeneralSound();

        StopRigidbodySoon(greens);
    }
}