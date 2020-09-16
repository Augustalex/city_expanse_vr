using UnityEngine;

public class Person : MonoBehaviour
{
    public AudioClip dyingSound;
    
    public float theta;
    public float radius = .05f;
    
    private Transform _homeTransform;
    private bool _started;
    private bool _targeted;

    void Update()
    {
        if (!_started) return;
        
        theta += .006f;
        
        var center = _homeTransform.transform.position;
        var x = Mathf.Cos(theta) * radius + center.x;
        var z = Mathf.Sin(theta) * radius + center.z;
        
        var yPosition = 1.2f - transform.localScale.y * .5f - .005f;
        var y = yPosition;
        
        transform.position = new Vector3(x, y, z);
    }

    public void SetHome(Transform homeTransform)
    {
        _homeTransform = homeTransform;
        _started = true;
    }

    public void Kill()
    {
        PlayDyingSound();
        Destroy(gameObject, 1);
    }

    public bool Targeted()
    {
        return _targeted;
    }

    public void SetAsTargeted()
    {
        _targeted = true;
    }

    public void PlayDyingSound()
    {
        GetComponent<AudioSource>().PlayOneShot(dyingSound);
    }
}
