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
    private int _size = 0;
    private FireworksEffect _fireworksEffect;
    private float _lifeStartedAt;
    private bool _innerCity;
    private bool _setup = false;
    private SmokeEffect _smokeEffect;

    void Awake()
    {
        // _fireworksEffect = GetComponentInChildren<FireworksEffect>();

        // _fireworksEffect.ActivatedHit += PlayUpgradeSound;
        _lifeStartedAt = Time.fixedTime;
    }

    void Start()
    {
        _smokeEffect = GetComponentInChildren<SmokeEffect>();
        Upgrade();
    }

    public bool GoodTimeToUpgrade()
    {
        return Time.fixedTime - _lifeStartedAt > 1f;
    }

    public void Upgrade()
    {
        if (!_setup)
        {
            SetupHouse(0, tinyHouseTemplates);
            _setup = true;
        }
        else if (_size == 0)
        {
            SetupHouse(1, largeHouseTemplates);
        }
        else if (_size == 1)
        {
            SetupHouse(2, megaHouseTemplates);
        }

        LaunchFromAbove();

        StartCoroutine(ActivateSmokeEffectSoon());

        IEnumerator ActivateSmokeEffectSoon()
        {
            yield return new WaitForSeconds(.05f);
            _smokeEffect.PlayOnNextHit();
        }
    }

    private void PlayUpgradeSound()
    {
        GetComponent<AudioSource>().PlayOneShot(upgradeSound, .015f * GameManager.MasterVolume);
    }

    private void LaunchFromAbove()
    {
        GetComponent<Rigidbody>().AddForce(Vector3.up * 2, ForceMode.Impulse);
    }

    private void SetupHouse(int size, GameObject[] templates)
    {
        if (_activeHouse)
        {
            Destroy(_activeHouse);
        }

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

    public void SetIsInnerCity()
    {
        _innerCity = true;
    }

    public bool IsInnerCityHouse()
    {
        return _innerCity;
    }
}