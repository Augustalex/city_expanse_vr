using System.Collections;
using System.Linq;
using UnityEngine;

public class Predator : MonoBehaviour
{
    public AudioClip eatingSound;

    private float _theta;
    private const float Radius = .05f;
    private Transform _homeTransform;
    private bool _started;
    private WorldPlane _worldPlane;
    private Block _block;
    private Person _target;
    private bool _huntStarted;
    private int _huntStep;
    private float _huntStartedAt;
    private Vector3 _startingPosition;
    private float _cooldownStart;
    private const float HuntLength = 1;

    void Start()
    {
        _worldPlane = WorldPlane.Get();
    }

    void Update()
    {
        if (!_started) return;

        if (_huntStarted)
        {
            if (_huntStep == 0)
            {
                var progress = Mathf.Clamp((Time.fixedTime - _huntStartedAt) / HuntLength, 0, 1.5f);
                transform.position = Vector3.Lerp(_startingPosition, _target.transform.position, progress);

                if (progress > 1f)
                {
                    _huntStep = 1;
                }
            }
            else if (_huntStep == 1)
            {
                _target.Kill();
                PlayEatingSoundSoon();

                _huntStep = 2;
            }
            else if (_huntStep == 2)
            {
                _cooldownStart = Time.fixedTime;
                _huntStep = 3;
            }
            else if (_huntStep == 3)
            {
                if ((Time.fixedTime - _cooldownStart) > 5)
                {
                    _huntStarted = false;
                }
            }
        }
        else
        {
            _theta += .006f;

            var center = _homeTransform.transform.position;
            var x = Mathf.Cos(_theta) * Radius + center.x;
            var z = Mathf.Sin(_theta) * Radius + center.z;

            var yPosition = 1.2f - transform.localScale.y * .5f - .005f;
            var y = yPosition;

            transform.position = new Vector3(x, y, z);

            if (Random.value < .01f)
            {
                var nearbyPersonTargets = _worldPlane
                    .GetNearbyBlocksWithinRange(_block.GetGridPosition(), 5f)
                    .Where(block =>
                    {
                        if (!block.OccupiedByHouse()) return false;

                        var house = block.GetOccupantHouse();
                        var personSpawn = house.GetComponent<PersonSpawn>();
                        if (!personSpawn) return false;
                        if (personSpawn.GetPerson() == null) return false;

                        return !personSpawn.GetPerson().Targeted();
                   })
                    .Select(block => block.GetOccupantHouse().GetComponent<PersonSpawn>().GetPerson())
                    .ToList();

                if (nearbyPersonTargets.Any())
                {
                    var nearbyPerson = nearbyPersonTargets.First();
                    nearbyPerson.SetAsTargeted();
                    _target = nearbyPerson;
                    _huntStartedAt = Time.fixedTime;
                    _startingPosition = transform.position;
                    _huntStep = 0;
                    _huntStarted = true;
                }
            }
        }
    }

    public void SetHome(Transform homeTransform)
    {
        _homeTransform = homeTransform;
        _started = true;
    }

    public void SetHomeBlock(Block block)
    {
        _block = block;
    }

    public void PlayEatingSoundSoon()
    {
        StartCoroutine(PlaySound());

        IEnumerator PlaySound()
        {
            yield return new WaitForSeconds(.5f);
            var audioSource = GetComponent<AudioSource>();
            if (audioSource)
            {
                audioSource.PlayOneShot(eatingSound);
            }
        }
    }
}