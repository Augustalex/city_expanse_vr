using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneToggle : MonoBehaviour
{
    public string sceneName = "Unnamed";
    
    private bool _on = false;
    
    public void On()
    {
        _on = true;
        gameObject.SetActive(true);
    }

    public void Off()
    {
        _on = false;
        gameObject.SetActive(false);
    }

    public bool IsOn()
    {
        return _on;
    }
}
