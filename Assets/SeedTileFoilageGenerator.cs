using UnityEngine;

public class SeedTileFoilageGenerator : MonoBehaviour
{
    public GameObject[] bushTemplates;

    void Start()
    {
        var bushCount = Random.Range(1, 2);

        for (int i = 0; i < bushCount; i++)
        {
            var randomBush = bushTemplates[Random.Range(0, bushTemplates.Length)];
            var bush = Instantiate(randomBush, transform, false);

            var randomDirection = Random.insideUnitCircle;
            var randomDirection3D = new Vector3(randomDirection.x, 0, randomDirection.y);
            var newRandomPosition = randomDirection3D * .4f;
            bush.transform.localPosition = new Vector3(newRandomPosition.x, 0, newRandomPosition.z);
        }
    }
}