using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChairsGeneration : MonoBehaviour
{
    [SerializeField] private GameObject _chairPrefab;
    [SerializeField] private Transform _parent;
    [SerializeField] private Transform _leftDownPilliar;
    [SerializeField] private Transform _leftUpPilliar;
    [SerializeField] private Transform _rightUpPilliar;
    [SerializeField] private Transform _rightDownPilliar;
    [SerializeField] private float _offsetFromPoints;
    [SerializeField] private int _rawCount;

    private Vector3 _leftDownPoint;
    private Vector3 _leftUpPoint;
    private Vector3 _upLeftPoint;
    private Vector3 _upRightPoint;
    private Vector3 _rightUpPoint;
    private Vector3 _rightDownPoint;
    private Vector3 _downRightPoint;
    private Vector3 _downLeftPoint;

    private float _offset;
    private Vector3 _startPosition;
    private float _startOffsetRaw;
    private float _endOffsetRaw;
    private float _height;

    private void Start()
    {
        _height = -1.62f;

        InitBorderPoints(_offsetFromPoints);
        _offset = Vector3.Distance(_leftDownPoint, _leftUpPoint) / (float) _rawCount;
        CreateChairs();
        
        _offsetFromPoints += 1.5f;
        InitBorderPoints(_offsetFromPoints);
        CreateChairs();
    }

    private void InitBorderPoints(float offset)
    {
        _leftDownPoint = _leftDownPilliar.position + new Vector3(-offset, _height, 0);
        _leftUpPoint = _leftUpPilliar.position + new Vector3(-offset, _height, 0);

        _upLeftPoint = _leftUpPilliar.position + new Vector3(0, _height, offset);
        _upRightPoint = _rightUpPilliar.position + new Vector3(0, _height, offset);

        _rightUpPoint = _rightUpPilliar.position + new Vector3(offset, _height, 0);
        _rightDownPoint = _rightDownPilliar.position + new Vector3(offset, _height, 0);

        _downRightPoint = _rightDownPilliar.position + new Vector3(0, _height, -offset);
        _downLeftPoint = _leftDownPilliar.position + new Vector3(0, _height, -offset);       
    } 

    private void CreateChairs()
    {
        for (int i = 0; i < _rawCount; i++)
        {
            GameObject chairLeft = Instantiate(_chairPrefab,
                _leftDownPoint + new Vector3(0, 0, _offset * i + _offset / 2),
                Quaternion.identity);
            chairLeft.name = "Chair Left " + (i + 1);
            chairLeft.transform.parent = _parent;

            GameObject chairUp = Instantiate(_chairPrefab, _upLeftPoint + new Vector3(_offset * i + _offset / 2, 0, 0),
                Quaternion.Euler(0, 90, 0));
            chairUp.name = "Chair Up " + (i + 1);
            chairUp.transform.parent = _parent;

            GameObject chairRight = Instantiate(_chairPrefab,
                _rightUpPoint + new Vector3(0, 0, -_offset * i - _offset / 2),
                Quaternion.Euler(0, 180, 0));
            chairRight.name = "Chair Right " + (i + 1);
            chairRight.transform.parent = _parent;

            GameObject chairDown = Instantiate(_chairPrefab,
                _downRightPoint + new Vector3(-_offset * i - _offset / 2, 0, 0),
                Quaternion.Euler(0, -90, 0));
            chairDown.name = "Chair Down " + (i + 1);
            chairDown.transform.parent = _parent;
        }
    }
}