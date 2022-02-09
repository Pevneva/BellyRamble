using UnityEngine;

public class Rope : MonoBehaviour
{
    private readonly float _breakDistance = 1.5f;
    private readonly float _destroyingDelay = 0.05f;
    private Rigidbody _connectedRigidbody;
    private Rigidbody _rigidbody;
    private GameObject _parent;

    private void Start()
    {
        _connectedRigidbody = GetComponent<HingeJoint>().connectedBody;
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (_connectedRigidbody == null || _rigidbody == null)
            return;
        
        if (Vector3.Distance(_connectedRigidbody.position, _rigidbody.position) > _breakDistance)
        {
            _parent = transform.parent.gameObject;
            Destroy(_parent, _destroyingDelay);
        }
    }
}
