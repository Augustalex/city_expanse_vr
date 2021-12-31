using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SmokeEffect : MonoBehaviour
{
    public AudioClip hitGroundSound;
    public bool activateOnHit = true;
    private bool _played;
    private bool _playOnNextHit;

    private void OnTriggerEnter(Collider other)
    {
        if (ShouldPlayOnHit() && !_played && other.GetComponentInParent<Block>() != null)
        {
            PlayAll();

            _played = true;
            _playOnNextHit = false;
        }
    }


    public void Play()
    {
        PlayAll();
    }
    
    public void PlayOnNextHit()
    {
        _playOnNextHit = true;
        _played = false;
    }

    public bool ShouldPlayOnHit()
    {
        return activateOnHit || _playOnNextHit;
    }
    
    private void PlayAll()
    {
        PlaySmokeEffect();
        PlayHitGroundSound();
    }

    private void PlaySmokeEffect()
    {
        var ps = GetComponent<ParticleSystem>();
        ps.Clear();
        ps.Play();
    }

    private void PlayHitGroundSound()
    {
        GetAudioSource().PlayOneShot(hitGroundSound, .02f * GameManager.MasterVolume);
    }

    private AudioSource GetAudioSource()
    {
        return GetComponentInParent<AudioSource>();
    }
}