using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(BlockRelative))]
public class GreensSpawn : MonoBehaviour
{
    public GameObject[] greensTemplates;
    public GameObject seedTemplate;
    
    private bool _grown;
    private GameObject _greens;
    private GameObject _seed;

    void Awake()
    {
        var rotation = new Vector3(0, Random.value * 360, 0);
        
        var greensTemplate = greensTemplates[Random.Range(0, greensTemplates.Length)];
        var greens = Instantiate(greensTemplate);
        greens.transform.SetParent(transform, false);
        greens.transform.Rotate(rotation);
        greens.SetActive(false);
        _greens = greens;

        var seed = Instantiate(seedTemplate);
        seed.transform.SetParent(transform, false);
        _seed = seed;
    }

    public void Grow()
    {
        if (_grown) return;
        _grown = true;
        
        _greens.SetActive(true);
        _seed.SetActive(false);
    }

    public bool IsGrown()
    {
        return _grown;
    }

    public void Cut()
    {
        _grown = false;
        
        _greens.SetActive(false);
        _seed.SetActive(true);
    }
}
