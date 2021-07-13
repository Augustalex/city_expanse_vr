using UnityEngine;

public class CloudMover : MonoBehaviour
{
    private Rigidbody _rigidbody;
    private WorldPlane _worldPlane;
    private bool _movingCloudWithMouse;
    private Camera _mainCamera;

    void Start()
    {
        _mainCamera = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();
        _rigidbody = GetComponent<Rigidbody>();
        _worldPlane = WorldPlane.Get();
    }

    void Update()
    {
        var force = .25f;
        var maxForce = .5f;
        var minForce = .001f;

        if (_movingCloudWithMouse)
        {
            if (Input.GetMouseButtonUp(0))
            {
                _movingCloudWithMouse = false;
            }
            else
            {
                RaycastHit hit;
                Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out hit, 1000.0f))
                {
                    var cloudPosition = transform.position;
                    var direction = hit.point - cloudPosition;
                    var flatDirection = new Vector3(direction.x, 0, direction.z);
                    var force2 = 6f * flatDirection.normalized * flatDirection.magnitude * .2f;

                    if (force2.magnitude < .1f)
                    {
                        _rigidbody.AddForce(-_rigidbody.velocity * .1f, ForceMode.Impulse);
                    }
                    else
                    {
                        _rigidbody.AddForce(force2 * Time.deltaTime, ForceMode.VelocityChange);
                    }
                    
                }
            }

            return;
        }

        if (IsOutsideLeft())
        {
            _rigidbody.AddForce(Vector3.right * force, ForceMode.Impulse);
        }

        if (IsOutsideRight())
        {
            _rigidbody.AddForce(Vector3.right * -force, ForceMode.Impulse);
        }

        if (IsOutsideBack())
        {
            _rigidbody.AddForce(Vector3.forward * force, ForceMode.Impulse);
        }

        if (IsOutsideFront())
        {
            _rigidbody.AddForce(Vector3.forward * -force, ForceMode.Impulse);
        }

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            GetComponent<Rigidbody>().AddForce(transform.right * -force, ForceMode.Impulse);
        }

        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            GetComponent<Rigidbody>().AddForce(transform.right * force, ForceMode.Impulse);
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            GetComponent<Rigidbody>().AddForce(transform.forward * -force, ForceMode.Impulse);
        }

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            GetComponent<Rigidbody>().AddForce(transform.forward * force, ForceMode.Impulse);
        }

        if (_rigidbody.velocity.magnitude > maxForce) return;

        if (_rigidbody.velocity.magnitude < minForce)
        {
            var direction = new Vector3((Random.value * 2) - 1, 0, (Random.value * 2) - 1);
            _rigidbody.AddForce(direction.normalized * .01f, ForceMode.Impulse);
        }

        if (_rigidbody.velocity.magnitude > maxForce)
        {
            _rigidbody.AddForce(_rigidbody.velocity * -(_rigidbody.velocity.magnitude - maxForce),
                ForceMode.VelocityChange);
        }
    }

    private bool IsOutsideFront()
    {
        return transform.position.z > _worldPlane.Top() - .2f;
    }

    private bool IsOutsideBack()
    {
        return transform.position.z < _worldPlane.Bottom() - .2f;
    }

    private bool IsOutsideRight()
    {
        return transform.position.x > _worldPlane.Right() - .2f;
    }

    private bool IsOutsideLeft()
    {
        return transform.position.x < _worldPlane.Left() - .2f;
    }

    public void CloudMouseDown()
    {
        _movingCloudWithMouse = true;
    }

    public bool IsMovingWithMouse()
    {
        return _movingCloudWithMouse;
    }
}