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
        Huge,
        Crazy
    }

    private bool _grown;
    private GameObject _greens;
    private GameObject _seed;
    private Vector3 _originalScale;
    private int _rain = 0;

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
            transform.localScale = _originalScale * 2f;
        }
        else if (size == TreeSize.Huge)
        {
            transform.localScale = _originalScale * 4f;
        }
        else if (size == TreeSize.Crazy)
        {
            transform.localScale = _originalScale * 8f;
        }
        else
        {
            transform.localScale = _originalScale;
        }
    }

    public void OnReceiveRain()
    {
        if (!_grown)
        {
            Grow();
        }
        else
        {
            if (_rain > 42) return;
        
            _rain += 1;
        
            if(_rain > 12) SetSize(TreeSize.Big);
            else if(_rain > 30) SetSize(TreeSize.Huge);
            else if(_rain == 42) SetSize(TreeSize.Crazy);
        }
    }
}
