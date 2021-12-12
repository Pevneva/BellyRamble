using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.Serialization;

public class CameraMover : MonoBehaviour
{
    [FormerlySerializedAs("_target")] [SerializeField] private Transform _horizontalTrackTarget;

    public bool IsBlockedMoving { get; set; }

    private bool _isHorizontalTracking;
    private Transform _trackTarget;
    private float _startY;
    private float _startZ;

    private void Start()
    {
        _isHorizontalTracking = true;
    }

    private void LateUpdate()
    {
        if (IsBlockedMoving)
            return;
        
        if (_isHorizontalTracking)
            HorizontalTrack(_horizontalTrackTarget);
        else
            Track(_trackTarget);
    }

    private void HorizontalTrack(Transform target)
    {
        Vector3 position = transform.position;
        var newPosition = new Vector3(target.position.x, position.y, position.z);
        transform.position = newPosition;
    }

    public void SetTrack(Transform target)
    {
        _isHorizontalTracking = false;
        _trackTarget = target;
        _startY = target.position.y;
        _startZ = target.position.z;
    }
    
    private void Track(Transform target)
    {
        Vector3 position = target.position;
        // var startY = target.position.y;
        // var startZ = target.position.z;

        Debug.Log("AAA-138 ");
        transform.position = new Vector3(position.x, position.y - _startY, position.z - _startZ);
    }
}
