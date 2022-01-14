using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;

public class ArrowRotation : MonoBehaviour
{
    [SerializeField] private RectTransform _rectTransform;

    private float _speed;
    private float _zRotation;
    private bool _isClockWise;

    private void Start()
    {
        Debug.Log("EEE _rectTransform.rotation.z : " + _rectTransform.rotation.z);
        _rectTransform.rotation = Quaternion.Euler(0, 0, 0);
        _speed = 1.5f;
        _zRotation = 0;
        _isClockWise = true;
    }

    private void Update()
    {
        if (_isClockWise)
        {
            _zRotation -= _speed;

            if (_zRotation < -180)
                _isClockWise = false;
        }
        else
        {
            _zRotation += _speed;

            if (_zRotation > 0)
                _isClockWise = true;
        }

        _rectTransform.rotation = Quaternion.Euler(0, 0, _zRotation);
    }
}