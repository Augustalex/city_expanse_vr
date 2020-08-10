using System;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(BlockRelative))]
public class GreensSpawn : MonoBehaviour
{
    public GameObject[] greensTemplates;
    private bool _grown;
    private Vector3 _originalScale;
    private Vector3 _originalPosition;
    private GameObject _greens;
    private GameObject _greensSmall;

    void Awake()
    {
        var rotation = new Vector3(0, Random.value * 360, 0);
        
        var greensTemplate = greensTemplates[Random.Range(0, greensTemplates.Length)];
        var greens = Instantiate(greensTemplate);
        greens.transform.SetParent(transform, false);
        greens.transform.Rotate(rotation);
        greens.SetActive(false);
        _greens = greens;
        
        var greensSmall = Instantiate(greensTemplate);
        greensSmall.transform.SetParent(transform, false);
        greensSmall.transform.Rotate(rotation);
        greensSmall.transform.localScale *= 1f;
        _greensSmall = greensSmall;
    }

    private void Start()
    {
        _originalScale = transform.localScale;
        _originalPosition = transform.position;
        
        transform.localScale = _originalScale * .5f;
        transform.position = _originalPosition;
    }

    public void Grow()
    {
        if (_grown) return;
        
        _grown = true;
        transform.localScale = _originalScale;
        transform.position = _originalPosition;
        
        _greens.SetActive(true);
        _greensSmall.SetActive(false);
    }

    public bool IsGrown()
    {
        return _grown;
    }
}
