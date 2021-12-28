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
    private float _timeShown;
    private DiscoveryManager _discoverManager;
    private bool _listenerSetup = false;
    private bool _beenShownBefore = false;

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
        _discoverManager = DiscoveryManager.Get();
        _discoverManager.NewDiscover += OnNewDiscovery;

        _toggles.AddRange(GetComponentsInChildren<SceneToggle>());
        ToggleAllOff();
    }

    private void OnNewDiscovery(DiscoveryManager.Discoverable discoverable)
    {
        return; // Disable "New discover whole screen modal" as it was annoying to players
        
        switch (discoverable)
        {
            case DiscoveryManager.Discoverable.House:
                DiscoveredHouse();
                break;
            case DiscoveryManager.Discoverable.BigHouse:
                DiscoveredBigHouse();
                break;
            case DiscoveryManager.Discoverable.Farm:
                DiscoveredFarm();
                break;
            case DiscoveryManager.Discoverable.Silo:
                DiscoveredSilo();
                break;
            case DiscoveryManager.Discoverable.Docks:
                DiscoveredDocks();
                break;
            case DiscoveryManager.Discoverable.CliffHouse:
                DiscoveredCliffHouse();
                break;
            case DiscoveryManager.Discoverable.ForrestShrine:
                DiscoveredForrestShrine();
                break;
        }
    }

    void Update()
    {
        if (!_anyToggleOn) return;

        var timeVisible = Time.time - _timeShown;
        if ((timeVisible > 2 && Input.GetMouseButtonDown(0))
            || Input.GetKeyDown(KeyCode.Return)
            || Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleAllOff();

            if (!_beenShownBefore)
            {
                MenuScene.Get().Show();
                _beenShownBefore = true;
            }
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
        _timeShown = Time.time;

        AudioSource.PlayClipAtPoint(newDiscovery, Vector3.zero, .6f);
    }

    public bool Visible()
    {
        return _anyToggleOn;
    }
}