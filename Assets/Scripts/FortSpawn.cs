using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FortSpawn : MonoBehaviour
{
    public GameObject[] tinyHouseTemplates;
    public AudioClip upgradeSound;
    
    private FireworksEffect _fireworksEffect;

    void Awake()
    {
        _fireworksEffect = GetComponentInChildren<FireworksEffect>();
        _fireworksEffect.ActivatedHit += PlayUpgradeSound;
        
        SetupHouse(tinyHouseTemplates);
        _fireworksEffect.SetHitBoxSize(5);
        
        StartCoroutine(ActivateFireworksSoon());
        LaunchFromAbove();

        IEnumerator ActivateFireworksSoon()
        {
            yield return new WaitForSeconds(.05f);
            _fireworksEffect.Activate();
        }
    }
    
    private void SetupHouse(GameObject[] templates)
    {
        var houseTemplate = templates[Random.Range(0, templates.Length)];
        Instantiate(houseTemplate, transform, false);
    }
    
    private void PlayUpgradeSound()
    {
        GetComponent<AudioSource>().PlayOneShot(upgradeSound, .5f);
    }
    
    private void LaunchFromAbove()
    {
        GetComponent<Rigidbody>().AddForce(Vector3.up * 2, ForceMode.Impulse);
    }
}
