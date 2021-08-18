using UnityEngine;

public class BlockSoundLibrary : MonoBehaviour
{
    private static BlockSoundLibrary _instance;

    public AudioClip basicSound;
    public AudioClip meteorSound;
    public AudioClip waterSound;
    public AudioClip placeItemSound;

    public enum BlockSound
    {
        Basic,
        Dig,
        RaiseLand,
        PlaceItem,
        Meteor,
        Water,
    }
    
    void Awake()
    {
        _instance = this;
    }
    
    public static BlockSoundLibrary Get()
    {
        return _instance;
    }

    public AudioClip GetSound(BlockSound blockSound)
    {
        switch (blockSound)
        {
            case BlockSound.Basic:
            case BlockSound.Dig:
            case BlockSound.RaiseLand:
                return basicSound;
            case BlockSound.Meteor:
                return meteorSound;
            case BlockSound.Water:
                return waterSound;
            case BlockSound.PlaceItem:
                return placeItemSound;
            default:
                return basicSound;
        }
    }
}