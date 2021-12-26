using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.Serialization;

public class CameraMover : MonoBehaviour
{
    [SerializeField] private Transform _trackTarget;
    [SerializeField] private float _trackingSpeed = 1.5f;

    private Vector3 _target;
    private Vector3 _currentPosition;
    private float _positionY;
    private float _positionZ;
    private bool _isHorizontalTracking;

    private void Start()
    {
        _isHorizontalTracking = true;
    }

    private void Update()
    {
        _currentPosition = Vector3.Lerp(transform.position, _target, _trackingSpeed * Time.deltaTime);
        transform.position = _currentPosition;

        _positionY = _isHorizontalTracking ? transform.position.y : _trackTarget.position.y + 16.5f;
        _positionZ = _isHorizontalTracking ? transform.position.z : _trackTarget.position.z - 10;
        _target = new Vector3(_trackTarget.position.x, _positionY, _positionZ);
    }

    public void SetKindMoving(bool isHorizontal)
    {
        _isHorizontalTracking = isHorizontal;
    }

    public void SetTarget(Transform newTarget)
    {
        _trackTarget = newTarget;
    }

}

//     public bool IsBlockedMoving { get; set; }
//
//     private bool _isHorizontalTracking;
//     private Transform _trackTarget;
//     private float _startY;
//     private float _startZ;
//     private float _startTargetY;
//     private float _startTargetZ;
//
//     private void Start()
//     {
//         // _isHorizontalTracking = false;
//         _isHorizontalTracking = true;
//         _startY = transform.position.y;
//         _startZ = transform.position.z;
//         _trackTarget = FindObjectOfType<Player>().transform;
//         _startTargetZ = _trackTarget.position.z;
//     }
//
//     private void LateUpdate()
//     {
//         if (IsBlockedMoving)
//             return;
//         
//         if (_isHorizontalTracking)
//             HorizontalTrack(_horizontalTrackTarget);
//         else
//             Track(_trackTarget);
//     }
//
//     private void HorizontalTrack(Transform target)
//     {
//         Vector3 position = transform.position;
//         var newPosition = new Vector3(target.position.x, position.y, position.z);
//         transform.position = newPosition;
//     }
//
//     public void SetTrack(Transform target)
//     {
//         _isHorizontalTracking = false;
//         _trackTarget = target;
//     }
//     
//     private void Track(Transform target)
//     {
//         Vector3 position = target.position;
//         // var startY = target.position.y;
//         // var startZ = target.position.z;
//
//         // Debug.Log("AAA-138 ");
//         // transform.position = new Vector3(position.x, position.y - _startY, position.z - _startZ);
//         
//         var newPosition = new Vector3(target.position.x, _startY, target.position.z - _startTargetZ  + transform.position.z);
//         transform.position = newPosition;
//     }
// }
