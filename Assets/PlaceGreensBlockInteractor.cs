using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceGreensBlockInteractor : BlockInteractor
{
    public GameObject greensBlockTemplate;

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
        if (blockComponent && blockComponent.IsInteractable() && blockComponent.blockType == Block.BlockType.Grass && blockComponent.IsVacant())
        {
            var greens = Instantiate(greensBlockTemplate);
            blockComponent.Occupy(greens);
            blockComponent.ShortFreeze();
            PlayGeneralSound();

            StopRigidbodySoon(greens);
        }
    }
}
