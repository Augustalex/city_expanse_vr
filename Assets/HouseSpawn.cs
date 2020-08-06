using System.Collections;
using UnityEngine;

[RequireComponent(typeof(BlockRelative))]
public class HouseSpawn : MonoBehaviour
{
    public GameObject[] tinyHouseTemplates;
    public GameObject[] largeHouseTemplates;
    public GameObject[] megaHouseTemplates;
    public AudioClip upgradeSound;
    
    private GameObject _activeHouse;
    private int _size;
    private FireworksEffect _fireworksEffect;
    private float _lifeStartedAt;

    void Awake()
    {
        SetupHouse(0, tinyHouseTemplates);
        _fireworksEffect = GetComponentInChildren<FireworksEffect>();

        _fireworksEffect.ActivatedHit += PlayUpgradeSound;
        
        _lifeStartedAt = Time.fixedTime;
    }
    
    public bool GoodTimeToUpgrade()
    {
        return Time.fixedTime - _lifeStartedAt > 1f;
    }

    public void Upgrade()
    {
        if (_size == 0)
        {
            SetupHouse(1, largeHouseTemplates);
            _fireworksEffect.SetHitBoxSize(2);
        }
        else if (_size == 1)
        {
            SetupHouse(2, megaHouseTemplates);
            _fireworksEffect.SetHitBoxSize(5);
        }

        StartCoroutine(ActivateFireworksSoon());
        LaunchFromAbove();

        IEnumerator ActivateFireworksSoon()
        {
            yield return new WaitForSeconds(.05f);
            _fireworksEffect.Activate();
        }
    }

    private void PlayUpgradeSound()
    {
        GetComponent<AudioSource>().PlayOneShot(upgradeSound, .5f);
    }

    private void LaunchFromAbove()
    {
        GetComponent<Rigidbody>().AddForce(Vector3.up * 2, ForceMode.Impulse);
    }
    
    private void SetupHouse(int size, GameObject[] templates)
    {
        Destroy(_activeHouse);

        var houseTemplate = templates[Random.Range(0, templates.Length)];
        var house = Instantiate(houseTemplate, transform, false);
                    
        _activeHouse = house;
        _size = size;
    }

    public bool IsSmall()
    {
        return _size == 0;
    }
    
    public bool IsBig()
    {
        return _size == 1;
    }
    
    public bool IsMegaBig()
    {
        return _size == 2;
    }

    public void SetToBig()
    {
        SetupHouse(1, largeHouseTemplates);
    }

    public void SetToMegaBig()
    {
        SetupHouse(2, megaHouseTemplates);
    }
}