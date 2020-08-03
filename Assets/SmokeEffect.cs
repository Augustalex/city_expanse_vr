using UnityEngine;

public class SmokeEffect : MonoBehaviour
{
    public AudioClip hitGroundSound;
    private bool _played;

    private void OnTriggerEnter(Collider other)
    {
        if (!_played && other.GetComponent<Block>() != null)
        {
            GetComponent<ParticleSystem>().Play();
            PlayHitGroundSound();
            
            _played = true;
        }
    }

    private void PlayHitGroundSound()
    {
        GetAudioSource().PlayOneShot(hitGroundSound, 1);
    }

    private AudioSource GetAudioSource()
    {
        return GetComponentInParent<AudioSource>();
    }
}