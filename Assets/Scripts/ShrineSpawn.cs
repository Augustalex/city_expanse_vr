using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShrineSpawn : MonoBehaviour
{
    public GameObject[] shrineTemplates;
    public AudioClip upgradeSound;
    
    private FireworksEffect _fireworksEffect;
    
    private static int _shrineCount = 0;

    void Awake()
    {
        _fireworksEffect = GetComponentInChildren<FireworksEffect>();
        _fireworksEffect.ActivatedHit += PlayUpgradeSound;
        
        SetupHouse(shrineTemplates);
        _fireworksEffect.SetHitBoxSize(5);
        
        StartCoroutine(ActivateFireworksSoon());
        LaunchFromAbove();

        IEnumerator ActivateFireworksSoon()
        {
            yield return new WaitForSeconds(.05f);
            _fireworksEffect.Activate();
        }
    }

    void Start()
    {
        if (_shrineCount == 0)
        {
            DiscoveryManager.Get().RegisterNewDiscover(DiscoveryManager.Discoverable.ForrestShrine);
        }

        _shrineCount += 1;
    }
    
    private void SetupHouse(GameObject[] templates)
    {
        var houseTemplate = templates[Random.Range(0, templates.Length)];
        Instantiate(houseTemplate, transform, false);
    }
    
    private void PlayUpgradeSound()
    {
        GetComponent<AudioSource>().PlayOneShot(upgradeSound, .015f * GameManager.MasterVolume);
    }
    
    private void LaunchFromAbove()
    {
        GetComponent<Rigidbody>().AddForce(Vector3.up * 2, ForceMode.Impulse);
    }
}
