using UnityEngine;

[RequireComponent(typeof(HouseSpawn))]
[RequireComponent(typeof(BlockRelative))]
public class PersonSpawn : MonoBehaviour
{
    public GameObject personTemplate;
    private Person _person;

    void Start()
    {
        if (FeatureToggles.Get().persons)
        {
            Populate();
        }
    }

    private void Populate()
    {
        var person = Instantiate(personTemplate);
        var personComponent = person.GetComponent<Person>();
        personComponent.SetHome(transform);

        _person = personComponent;
    }

    public Person GetPerson()
    {
        var notCreatedOrHasBeenDestroyed = _person == null;
        if (notCreatedOrHasBeenDestroyed) _person = null;

        return _person;
    }
}