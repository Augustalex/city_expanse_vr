using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class RaiseLandBlockInteractor : BlockInteractor
{
    public GameObject grassBlockTemplate;

    private void OnTriggerEnter(Collider other)
    {
        Interact(other.gameObject);
    }

    public override void Interact(GameObject other)
    {
        if (!IsActivated()) return;

        var blockComponent = other.gameObject.GetComponent<Block>();
        if (
            blockComponent &&
            blockComponent.blockType == Block.BlockType.Grass &&
            blockComponent.IsVacant() &&
            blockComponent.IsInteractable()
        )
        {
            var grass = Instantiate(grassBlockTemplate);
            var transformRotation = grass.transform.rotation;
            grass.transform.rotation = Quaternion.Euler(transformRotation.x, (Random.Range(0, 5) * 60), transformRotation.z);
            var grassBlock = grass.GetComponentInChildren<Block>();
            blockComponent.PlaceOnTopOfSelf(grassBlock, grass);
            PlayGeneralSound();
        }
    }
}