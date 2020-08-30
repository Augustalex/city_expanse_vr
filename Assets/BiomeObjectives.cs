using System;
using System.Collections.Generic;
using UnityEngine;

public class BiomeObjectives : MonoBehaviour
{
    private float _delta;
    private WorldPlane _worldPlane;
    private List<Biome> _biomes;


    private void Start()
    {
        _worldPlane = WorldPlane.Get();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            BlockHighlighter.Highlight(_biomes[0].GetBlocks());
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            BlockHighlighter.Highlight(_biomes[1].GetBlocks());
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            BlockHighlighter.Highlight(_biomes[2].GetBlocks());
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            BlockHighlighter.Highlight(_biomes[3].GetBlocks());
        }
        
        if (Input.GetKeyDown(KeyCode.Space))
        {
            EstablishBiomePerk();
        }
        return;
        if (_delta > 2 * 60)
        {
            EstablishBiomePerk();

            _delta = 0;
        }

        _delta += Time.deltaTime;
    }

    private void EstablishBiomePerk()
    {
        DecideBiomePerkBuilding();
    }
 
    private void DecideBiomePerkBuilding()
    {
        _biomes = _worldPlane.GetBiomes();
    }
}