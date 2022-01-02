using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AvailablePlansTitle : MonoBehaviour
{
    private TMP_Text _text;

    void Awake()
    {
        _text = GetComponent<TMP_Text>();
    }

    public void SetNoContentTitle()
    {
        _text.text = "No plans available yet";
    }

    public void SetMainTitle()
    {
        _text.text = "Available plans";
    }
}
