using UnityEngine;

public class RaiseLandBlockInteractor : BlockInteractor
{
    public GameObject grassBlockTemplate;

    private void OnTriggerEnter(Collider other)
    {
        if (!IsActivated()) return;

        var blockComponent = other.gameObject.GetComponent<Block>();
        if (!blockComponent || blockComponent.blockType != Block.BlockType.Grass || !blockComponent.Vacant() || !blockComponent.IsInteractable()) return;

        var grass = Instantiate(grassBlockTemplate);
        var grassBlock = grass.GetComponentInChildren<Block>();
        blockComponent.PlaceOnTopOfSelf(grassBlock, grass);
        PlayGeneralSound();
    }
}
