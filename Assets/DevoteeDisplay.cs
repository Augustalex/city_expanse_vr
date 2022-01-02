using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DevoteeDisplay : MonoBehaviour
{
    private TMP_Text _text;

    void Start()
    {
        _text = GetComponentInChildren<TMP_Text>();
    }

    public void SetCount(int count)
    {
        _text.text = $"{count} devotees";
    }
}
