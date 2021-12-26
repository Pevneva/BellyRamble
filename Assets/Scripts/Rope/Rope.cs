using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rope : MonoBehaviour
{
    // private Vector3 _startScale;
    private Rigidbody _connectedRigidbody;
    private Rigidbody _rigidbody;
    private GameObject _parent;

    private void Start()
    {
        _connectedRigidbody = GetComponent<HingeJoint>().connectedBody;
        _rigidbody = GetComponent<Rigidbody>();

        // _startScale = transform.localScale;

        // Debug.Log("AAA-3 _startScale : " + _startScale);
    }

    // private void Update()
    // {
    //     if (Vector3.Distance(_connectedRigidbody.position, _rigidbody.position) > 1.5f)
    //     {
    //         _parent = transform.parent.gameObject;
    //         Debug.Log("SASA _parent: " + _parent);
    //         Destroy(gameObject);
    //         Invoke(nameof(DestroyParent), 1);
    //     } ;
    //     
    //     // Debug.Log("DAD transform.rotation : " + transform.rotation.eulerAngles);
    //     // if (transform.rotation.eulerAngles.y > 90)// || transform.rotation.eulerAngles.y < -90)
    //     //     Destroy(transform.parent.gameObject);
    // }

    private void DestroyParent()
    {
        Debug.Log("SASA DESTROY _parent!" );
        Destroy(_parent);
    }
}
