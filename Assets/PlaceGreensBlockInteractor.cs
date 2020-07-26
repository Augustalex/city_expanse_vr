using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceGreensBlockInteractor : BlockInteractor
{
    public GameObject greensBlockTemplate;

    private void OnTriggerEnter(Collider other)
    {
        if (!IsActivated()) return;

        var blockComponent = other.gameObject.GetComponent<Block>();
        if (blockComponent && blockComponent.IsInteractable() && blockComponent.blockType == Block.BlockType.Grass && blockComponent.Vacant())
        {
            var greens = Instantiate(greensBlockTemplate);
            blockComponent.Occupy(greens);
            blockComponent.ShortFreeze();
            PlayGeneralSound();

            StopRigidbodySoon(greens);
        }
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
}
