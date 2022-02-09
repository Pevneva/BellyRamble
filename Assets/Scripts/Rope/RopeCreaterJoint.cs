using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopeCreaterJoint : MonoBehaviour
{
    [SerializeField] private GameObject _partOfRope;
    [SerializeField] private Rigidbody _startPointRB;
    [SerializeField] private Rigidbody _endPointRB;
    [SerializeField] private Transform _parent;
    [SerializeField] private int _segments;

    private readonly Vector3 _endOffsetPosition = new Vector3(0.1f, 0, 0.05f);
    private readonly float _scaleKoef = 1.3f;
    private float _stepOffset;
    private Rigidbody _previousJoint;
    
    private void Start()
    {
        _previousJoint = null;
        
        var startCenterPosition = _startPointRB.gameObject.transform.position;
        var localScaleSegment = _partOfRope.transform.localScale;
        _stepOffset = localScaleSegment.y / 4;
        var startPosition = new Vector3(startCenterPosition.x, startCenterPosition.y + _stepOffset, startCenterPosition.z);
        var posX = startPosition.x;
        var posY = startPosition.y;
        var posZ = startPosition.z;

        for (int i = 0; i < _segments; i++)
        {
            posZ -= localScaleSegment.y * _scaleKoef;
            var position = new Vector3(posX, posY, posZ);
            var rope = Instantiate(_partOfRope, position, Quaternion.Euler(90, 0, 0));
            rope.transform.parent = _parent;
            var joint = rope.GetComponent<ConfigurableJoint>();

            if (i == 0)
            {
                joint.connectedBody = _startPointRB;
                _previousJoint = joint.GetComponent<Rigidbody>();
            }
            else
            {
                joint.connectedBody = _previousJoint;
                _previousJoint = joint.GetComponent<Rigidbody>();
                if (i == _segments - 1)
                {
                    joint.GetComponent<Rigidbody>().isKinematic = true;
                    var endSegmentSize = new Vector3(localScaleSegment.x, localScaleSegment.x,
                        localScaleSegment.z);
                    joint.transform.localScale = endSegmentSize;
                    joint.transform.position += _endOffsetPosition;
                }
            }
        }
    }
}
