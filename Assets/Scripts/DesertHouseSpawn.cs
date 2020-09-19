using UnityEngine;

public class DesertHouseSpawn : MonoBehaviour
{
    public GameObject[] tinyHouseTemplates;
    
    private GameObject _activeHouse;
    
    void Awake()
    {
        SetupHouse(tinyHouseTemplates);
    }

    private void SetupHouse(GameObject[] templates)
    {
        Destroy(_activeHouse);

        var houseTemplate = templates[Random.Range(0, templates.Length)];
        var house = Instantiate(houseTemplate, transform, false);
                    
        _activeHouse = house;
    }
}
