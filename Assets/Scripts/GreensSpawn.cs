using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(BlockRelative))]
public class GreensSpawn : MonoBehaviour
{
    public GameObject[] greensTemplates;
    public GameObject seedTemplate;

    public enum TreeSize
    {
        Small,
        Big,
        Huge
    }

    private bool _grown;
    private GameObject _greens;
    private GameObject _seed;
    private Vector3 _originalScale;

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

        _originalScale = transform.localScale;
    }

    public void Grow()
    {
        if (_grown) return;
        _grown = true;
        
        _greens.SetActive(true);
        // _seed.SetActive(false);
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

    public void SetSize(TreeSize size)
    {
        if (size == TreeSize.Big)
        {
            transform.localScale = _originalScale * 1.25f;
        }
        else if (size == TreeSize.Huge)
        {
            transform.localScale = _originalScale * 1.8f;
        }
        else
        {
            transform.localScale = _originalScale;
        }
    }
}
