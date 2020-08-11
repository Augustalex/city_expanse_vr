using UnityEngine;

public class WoodcutterSpawn : MonoBehaviour
{
    public GameObject[] woodcutterTemplates;
    private GameObject _woodcutter;

    void Start()
    {
        var rotation = new Vector3(0, Random.value * 360, 0);

        var woodcutterTemplate = woodcutterTemplates[Random.Range(0, woodcutterTemplates.Length)];
        var woodcutter = Instantiate(woodcutterTemplate);
        woodcutter.transform.SetParent(transform, false);
        woodcutter.transform.Rotate(rotation);
        _woodcutter = woodcutter;
    }
}
