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

    private Vector3 _currentPosition;
    private float _offsetSegment;
    private Rigidbody _previousJoint;
    
    private void Start()
    {
        _previousJoint = null;
        
        var startCenterPosition = _startPointRB.gameObject.transform.position;
        var localScaleSegment = _partOfRope.transform.localScale;
        var startPosition = new Vector3(startCenterPosition.x, startCenterPosition.y + localScaleSegment.y / 4, startCenterPosition.z);
        
        // _offsetSegment = ;
        // _offsetSegment = _partOfRope.gameObject.GetComponent<CapsuleCollider>().height / 2;
        _currentPosition = startPosition;
        
        var wholeDistanceRb = Vector3.Distance(startPosition, _endPointRB.gameObject.transform.position);
        var segmentDistanceRb = (float) wholeDistanceRb / _segments;

        var posX = startPosition.x;
        var posY = startPosition.y;
        var posZ = startPosition.z;

        Debug.Log("localScaleSegment : " + localScaleSegment);
        Debug.Log("_segments : " + _segments);
        Debug.Log("wholeDistanceRB : " + wholeDistanceRb);
        Debug.Log("segmentDistanceRB : " + segmentDistanceRb);

        for (int i = 0; i < _segments; i++)
        {
            posZ -= localScaleSegment.y * 1.3f ;
            var position = new Vector3(posX, posY, posZ);
            var rope = Instantiate(_partOfRope, position, Quaternion.Euler(90, 0, 0));
            rope.transform.parent = _parent;
            var joint = rope.GetComponent<ConfigurableJoint>();
            // var joint = rope.GetComponent<SpringJoint>();

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
                    joint.transform.position += new Vector3(0.1f, 0, 0.05f);
                }
            }
            
            // _endPointRB.GetComponent<HingeJoint>().connectedBody = _previousJoint;

            
            

            // posZ += _offsetSegment;
        }
    }
}
