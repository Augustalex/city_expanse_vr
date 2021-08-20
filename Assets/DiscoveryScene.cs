using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DiscoveryScene : MonoBehaviour
{
    private static DiscoveryScene _instance;
    private AudioSource _mainAudioSource;

    private List<SceneToggle> _toggles = new List<SceneToggle>();

    public AudioClip newDiscovery;
    private bool _anyToggleOn;
    private GameObject _discoveryCamera;

    public static DiscoveryScene Get()
    {
        return _instance;
    }
    
    void Awake()
    {
        _instance = this;
        _mainAudioSource = Camera.main.GetComponent<AudioSource>();
        _discoveryCamera = GetComponentInChildren<DiscoveryCamera>().gameObject;
    }
    
    void Start()
    {
        _toggles.AddRange(GetComponentsInChildren<SceneToggle>());
        ToggleAllOff();
    }

    void Update()
    {
        if (!_anyToggleOn) return;
        
        if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleAllOff();
        }
    }

    private void ToggleAllOff()
    {
        _anyToggleOn = false;
        _discoveryCamera.SetActive(false);
        _toggles.ForEach(t => t.Off());
    }

    public void DiscoveredHouse()
    {
        StartCoroutine(ToggleSoon(FindScene("House"), 1.5f));
    }
    
    private IEnumerator ToggleSoon(SceneToggle toggle, float delay)
    {
        yield return new WaitForSeconds(delay);
        ToggleAllOff();
        ToggleToggle(toggle);
    }
    
    public void DiscoveredBigHouse()
    {
        StartCoroutine(ToggleSoon(FindScene("BigHouse"), 1.5f));
    }
    
    public void DiscoveredCliffHouse()
    {
        StartCoroutine(ToggleSoon(FindScene("CliffHouse"), 1.5f));
    }
    
    public void DiscoveredFarm()
    {
        StartCoroutine(ToggleSoon(FindScene("Farm"), 1.5f));
    }
    
    public void DiscoveredSilo()
    {
        StartCoroutine(ToggleSoon(FindScene("Silo"), 1.5f));
    }
    
    public void DiscoveredDocks()
    {
        StartCoroutine(ToggleSoon(FindScene("Docks"), 1.5f));
    }
    
    public void DiscoveredForrestShrine()
    {
        StartCoroutine(ToggleSoon(FindScene("ForrestShrine"), 1.5f));
    }

    private SceneToggle FindScene(string sceneName)
    {
        return _toggles.First(toggle => toggle.sceneName == sceneName);
    }

    private void ToggleToggle(SceneToggle toggle)
    {
        _anyToggleOn = true;
        _discoveryCamera.SetActive(true);
        toggle.On();
        
        AudioSource.PlayClipAtPoint(newDiscovery, Vector3.zero, .6f);
    }
}
