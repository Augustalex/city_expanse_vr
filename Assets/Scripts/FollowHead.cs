using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowHead : MonoBehaviour
{
    public Transform followPoint;
    public ClosedFistDetector closedFistDetector;

    private bool _paletteInSight;
    private BlockInteractionPalette _activePalette;

    void Start()
    {
        closedFistDetector.OpenFist += ShowActivePalette;
        closedFistDetector.CloseFist += HideActivePalette;
    }
    
    void Update()
    {
        transform.SetPositionAndRotation(followPoint.position, followPoint.rotation);
    }

    private void ShowActivePalette()
    {
        if (_paletteInSight)
        {
            _activePalette.Show();
        }
    }
    
    private void HideActivePalette()
    {
        if (_paletteInSight)
        {
            _activePalette.Hide();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        var palette = other.GetComponent<BlockInteractionPalette>();
        if (palette)
        {
            _activePalette = palette;
            _paletteInSight = true;
            
            if (closedFistDetector.IsOpen())
            {
                palette.Show();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        var palette = other.GetComponent<BlockInteractionPalette>();
        if (palette)
        {
            _activePalette = null;
            _paletteInSight = false;
            
            palette.Hide();
        }
    }
}
