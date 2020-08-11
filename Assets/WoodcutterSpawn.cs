using System.Collections;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(BlockRelative))]
[RequireComponent(typeof(AudioSource))]
public class WoodcutterSpawn : MonoBehaviour
{
    public GameObject[] woodcutterTemplates;
    public AudioClip cuttingDownTree;

    private Block _block;
    private AudioSource _audioSource;
    private WorldPlane _worldPlane;
    private CityWoodcutters _cityWoodcutters;

    void Start()
    {
        _worldPlane = WorldPlane.Get();
        _block = GetComponent<BlockRelative>().block;
        _audioSource = GetComponent<AudioSource>();
        _cityWoodcutters = CityWoodcutters.Get();

        var rotation = new Vector3(0, Random.value * 360, 0);

        var woodcutterTemplate = woodcutterTemplates[Random.Range(0, woodcutterTemplates.Length)];
        var woodcutter = Instantiate(woodcutterTemplate);
        woodcutter.transform.SetParent(transform, false);
        woodcutter.transform.Rotate(rotation);
    }

    void Update()
    {
        if (Random.value < .001f)
        {
            Woodcut();
        }
    }

    private void Woodcut()
    {
        var nearbyTreeOrNull = _worldPlane.GetNearbyBlocksWithinRange(_block.GetGridPosition(), 2f)
            .Where(block => block.OccupiedByGreens())
            .OrderBy(_ => Random.value)
            .FirstOrDefault();

        if (nearbyTreeOrNull != null)
        {
            PlayWoodcutterSound();
            StartCoroutine(DoSoon());
            _cityWoodcutters.RequestStorageSpace();

            IEnumerator DoSoon()
            {
                yield return new WaitForSeconds(1.5f);
                if (nearbyTreeOrNull != null)
                {
                    nearbyTreeOrNull.GetOccupantGreens().Cut();

                    _cityWoodcutters.StoreWood();
                }
            }
        }
    }

    private void PlayWoodcutterSound()
    {
        _audioSource.PlayOneShot(cuttingDownTree);
    }
}