using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class CameraMover : MonoBehaviour
{
    [SerializeField] private Transform _target;

    private void Update()
    {
        HorizontalTrack(_target);
    }

    private void HorizontalTrack(Transform target)
    {
        var newPosition = new Vector3(_target.position.x, transform.position.y, transform.position.z);
        transform.position = newPosition;
    }
}
