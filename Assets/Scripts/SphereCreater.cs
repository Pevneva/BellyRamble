using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphereCreater : MonoBehaviour
{
    [SerializeField] private GameObject _spherePrefab;
    [SerializeField] private Transform _parent;
    [SerializeField] private Transform _startPilliar;
    [SerializeField] private float _offsetPilliar;
    [SerializeField] private float _heightUp;
    [SerializeField] private float _heightDown;

    private Vector3 _currentPosition;
    private float _deltaHeight;

    private void Start()
    {
        Vector3 pilliarPosition = _startPilliar.position;
        Vector3 offsetVector = new Vector3(_offsetPilliar, 0, _offsetPilliar);
        _deltaHeight = _heightUp - _heightDown;

        _currentPosition =
            new Vector3(pilliarPosition.x, _heightUp, pilliarPosition.z) + offsetVector;

        CreateTwoSpheres(_currentPosition);
        _currentPosition += new Vector3(0, 0, 8.3f);

        CreateTwoSpheres(_currentPosition);
        _currentPosition += new Vector3(8f, 0, 0);

        CreateTwoSpheres(_currentPosition);
        _currentPosition += new Vector3(0, 0, -8.3f);

        CreateTwoSpheres(_currentPosition);
        _currentPosition += new Vector3(-8f, 0, 0);
    }

    private void CreateSphere(Vector3 position)
    {
        GameObject sphere = Instantiate(_spherePrefab, position, Quaternion.identity);
        sphere.transform.parent = _parent;
    }

    private void CreateTwoSpheres(Vector3 position)
    {
        CreateSphere(position);
        CreateSphere(position - new Vector3(0, _deltaHeight, 0));
    }
}