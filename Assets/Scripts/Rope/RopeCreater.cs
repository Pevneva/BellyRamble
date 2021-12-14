using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopeCreater : MonoBehaviour
{
    [SerializeField] private GameObject _partOfRope;
    // [SerializeField] private Rigidbody _startPointRB;
    // [SerializeField] private Rigidbody _endPointRB;
    [SerializeField] private Rigidbody _sphereLeftDownRB_UP;
    [SerializeField] private Rigidbody _sphereLeftUpRB_UP;
    [SerializeField] private Rigidbody _sphereRightUpRB_UP;
    [SerializeField] private Rigidbody _sphereRightDownRB_UP;
    [SerializeField] private Rigidbody _sphereLeftDownRB_DOWN;
    [SerializeField] private Rigidbody _sphereLeftUpRB_DOWN;
    [SerializeField] private Rigidbody _sphereRightUpRB_DOWN;
    [SerializeField] private Rigidbody _sphereRightDownRB_DOWN;
    [SerializeField] private Transform _parent;
    [SerializeField] private int _segments;

    // private Vector3 _currentPosition;
    private float _offsetSegment;
    private Rigidbody _previousJoint;
    private float _deltaHeight;

    private void Start()
    {
        ShowDistance();

        GameObject parent1 = new GameObject();
        GameObject parent2 = new GameObject();
        GameObject parent3 = new GameObject();
        GameObject parent4 = new GameObject();
        GameObject parent5 = new GameObject();
        GameObject parent6 = new GameObject();
        GameObject parent7 = new GameObject();
        GameObject parent8 = new GameObject();
        parent1.transform.parent = _parent;
        parent2.transform.parent = _parent;
        parent3.transform.parent = _parent;
        parent4.transform.parent = _parent;
        parent5.transform.parent = _parent;
        parent6.transform.parent = _parent;
        parent7.transform.parent = _parent;
        parent8.transform.parent = _parent;
        parent1.name = "Left";
        parent2.name = "Down";
        parent3.name = "Right";
        parent4.name = "Up";
        parent5.name = "Left 2";
        parent6.name = "Down 2";
        parent7.name = "Right 2";
        parent8.name = "Up 2";
        
        CreateRope(_sphereLeftUpRB_UP, _sphereLeftDownRB_UP, true, false, parent1.transform);
        CreateRope(_sphereLeftDownRB_UP, _sphereRightDownRB_UP, false, true, parent2.transform);
        CreateRope(_sphereRightDownRB_UP, _sphereRightUpRB_UP, true, true, parent3.transform);
        CreateRope(_sphereRightUpRB_UP, _sphereLeftUpRB_UP, false, false, parent4.transform); 
        //
        CreateRope(_sphereLeftUpRB_DOWN, _sphereLeftDownRB_DOWN, true, false, parent1.transform);
        CreateRope(_sphereLeftDownRB_DOWN, _sphereRightDownRB_DOWN, false, true, parent2.transform);
        CreateRope(_sphereRightDownRB_DOWN, _sphereRightUpRB_DOWN, true, true, parent3.transform);
        CreateRope(_sphereRightUpRB_DOWN, _sphereLeftUpRB_DOWN, false, false, parent4.transform);
    }

    private void ShowDistance()
    {
        // Debug.Log("DOS 1 : " + Vector3.Distance(_sphereLeftUpRB.gameObject.transform.position, _sphereLeftDownRB.gameObject.transform.position));
        // Debug.Log("DOS 2 : " + Vector3.Distance(_sphereLeftDownRB.gameObject.transform.position, _sphereRightDownRB.gameObject.transform.position));
        // Debug.Log("DOS 3 : " + Vector3.Distance(_sphereRightDownRB.gameObject.transform.position, _sphereRightUpRB.gameObject.transform.position));
        // Debug.Log("DOS 4 : " + Vector3.Distance(_sphereRightUpRB.gameObject.transform.position, _sphereLeftUpRB.gameObject.transform.position));
    }

    private void CreateRope(Rigidbody startPoint, Rigidbody endPoint, bool isVertical, bool isPositive, Transform parent)
    {
        _previousJoint = null;

        var localScaleSegment = _partOfRope.transform.localScale;
        var startPosition = startPoint.gameObject.transform.position;
        var endPosition = endPoint.gameObject.transform.position;
        var wholeDistanceRb = Vector3.Distance(startPosition, endPosition);
        var segmentDistanceRb = (float) wholeDistanceRb / _segments;

        var posX = startPosition.x;
        var posY = startPosition.y;
        var posZ = startPosition.z;

        // Debug.Log("localScaleSegment : " + localScaleSegment);
        // Debug.Log("_segments : " + _segments);
        // Debug.Log("wholeDistanceRB : " + wholeDistanceRb);
        // Debug.Log("segmentDistanceRB : " + segmentDistanceRb);

        float offsetKoef = 1.5f;
        for (int i = 0; i < _segments; i++)
        {
            if (isVertical)
                posZ = isPositive ? posZ + localScaleSegment.y * offsetKoef : posZ - localScaleSegment.y * offsetKoef;
            else
                posX = isPositive ? posX + localScaleSegment.y * offsetKoef : posX - localScaleSegment.y * offsetKoef;
            // posZ -= localScaleSegment.y * 1.4f;

            var position = new Vector3(posX, posY, posZ);
            var rope = Instantiate(_partOfRope, position, 
                isVertical ? Quaternion.Euler(90, 0, 0) : Quaternion.Euler(0, 0, 90));
            rope.transform.parent = parent;
            //
            // var rope2 = Instantiate(_partOfRope, position + new Vector3(0, -_deltaHeight, 0), 
            //     isVertical ? Quaternion.Euler(90, 0, 0) : Quaternion.Euler(0, 0, 90));
            // rope2.transform.parent = rope.transform;
            
            var hinge = rope.GetComponent<HingeJoint>();

            if (i == 0)
            {
                hinge.connectedBody = startPoint;
                _previousJoint = hinge.GetComponent<Rigidbody>();
            }
            else
            {
                hinge.connectedBody = _previousJoint;
                _previousJoint = hinge.GetComponent<Rigidbody>();
                if (i == _segments - 1)
                {
                    endPoint.gameObject.GetComponent<HingeJoint>().connectedBody = rope.GetComponent<Rigidbody>();
                }
            }
        }
    }

    private void ChangeNextPosition(float localScaleSegment)
    {
    }

    private void CreateRopeOld(Rigidbody startPoint, Rigidbody endPoint)
    {
        _previousJoint = null;

        var startCenterPosition = startPoint.gameObject.transform.position;
        var localScaleSegment = _partOfRope.transform.localScale;
        var startPosition = new Vector3(startCenterPosition.x, startCenterPosition.y + localScaleSegment.y / 4,
            startCenterPosition.z);

        var wholeDistanceRb = Vector3.Distance(startPosition, endPoint.gameObject.transform.position);
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
            posZ -= localScaleSegment.y * 1.3f;
            var position = new Vector3(posX, posY, posZ);
            var rope = Instantiate(_partOfRope, position, Quaternion.Euler(90, 0, 0));
            rope.transform.parent = _parent;
            var hinge = rope.GetComponent<HingeJoint>();

            if (i == 0)
            {
                hinge.connectedBody = startPoint;
                _previousJoint = hinge.GetComponent<Rigidbody>();
            }
            else
            {
                hinge.connectedBody = _previousJoint;
                _previousJoint = hinge.GetComponent<Rigidbody>();
                if (i == _segments - 1)
                {
                    hinge.GetComponent<Rigidbody>().isKinematic = true;
                    var endSegmentSize = new Vector3(localScaleSegment.x, localScaleSegment.x,
                        localScaleSegment.z);
                    hinge.transform.localScale = endSegmentSize;
                    hinge.transform.position += new Vector3(0.1f, 0, 0.05f);
                }
            }
        }
    }
}