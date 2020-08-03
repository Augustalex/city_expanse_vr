using UnityEngine;

public class SmokeEffect : MonoBehaviour
{
    private bool _played;

    private void OnTriggerEnter(Collider other)
    {
        if (!_played && other.GetComponent<Block>() != null)
        {
            GetComponent<ParticleSystem>().Play();
            _played = true;
        }
    }
}