using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class FoodGeneration : MonoBehaviour
{
    [SerializeField] private GridObject[] _foodPrefabs;
    [SerializeField] private GridObject[] _primaryFoodPrefabs;
    [SerializeField] private float _cellSize;
    [SerializeField] private Transform _leftDownPoint;
    [SerializeField] private Transform _rightUpPoint;
    [SerializeField] private Transform _ground;

    private List<Vector3Int> _collisionsMatrix = new List<Vector3Int>();
    private float _generationHeight;
    private float _delayCreating;
    
    private void Start()
    {
        _generationHeight = _ground.position.y + 0.2f;
        FillArea(_leftDownPoint.position, _rightUpPoint.position, 1);
        
        var distanceX = Vector3.Distance(_leftDownPoint.position,
            new Vector3(_rightUpPoint.position.x, _leftDownPoint.position.y, _leftDownPoint.position.z));
    }

    private void Update()
    {
        FillArea(_leftDownPoint.position, _rightUpPoint.position, 1);
    }

    private void FillArea(Vector3 leftDownPoint, Vector3 rightUpPoint, float delay)
    {
        var startPoint = WorldToGridPosition(leftDownPoint);
        var endPoint = WorldToGridPosition(rightUpPoint);
        
        // Debug.Log("startPoint : " + startPoint);
        // Debug.Log("endPoint : " + endPoint);
    
        for (int x = startPoint.x ; x < endPoint.x ; x++)
        {
            for (int z = startPoint.z + 1; z < endPoint.z ; z++)
            {
                TryCreate(_foodPrefabs[0],new Vector3Int(x, 0, z));
                TryCreate(_primaryFoodPrefabs[0], new Vector3Int(x, 0, z));
            }
        }
    }

    private void AddFood()
    {
        
    }
    
    private void TryCreate(GridObject template, Vector3Int gridPosition)
    {
        if (_collisionsMatrix.Contains(gridPosition))
            return;
        else
            _collisionsMatrix.Add(gridPosition);
        
        // var template = GetRandomTemplate();
        
        if (template == null)
            return;
        
        var position = GridToWorldPosition(gridPosition);
        var offsetX = Random.Range(-0.3f, 0.3f);
        var offsetZ = Random.Range(-0.3f, 0.3f);
        var rotateY = Random.Range(0, 180);
        Quaternion quaternionY = Quaternion.Euler(0, rotateY, 0);

        position = position + new Vector3(offsetX + 0.25f, -1, offsetZ + 0.35f);
        Debug.Log("position : " + position);

        if (template.Chance > Random.Range(0, 100))
        {
            var gridObject = Instantiate(template, position, quaternionY, transform);
            gridObject.Died += OnDied;
            gridObject.SetCoordinads(gridPosition);
        }
    }

    
    private Vector3 GridToWorldPosition(Vector3Int gridPosition)
    {
        return new Vector3(
            gridPosition.x * _cellSize,
            gridPosition.y * _cellSize,
            gridPosition.z * _cellSize);
    }
    
    private Vector3Int WorldToGridPosition(Vector3 worldPosition)
    {
        return new Vector3Int(
            (int)(worldPosition.x / _cellSize),
            (int)(worldPosition.y / _cellSize),
            (int)(worldPosition.z / _cellSize));
    }

    private void OnDied(Vector3Int gridPosition)
    {
        TryRemoveFromCollisionsMatrix(gridPosition);
    }

    public void TryRemoveFromCollisionsMatrix(Vector3Int gridPosition)
    {
        if (_collisionsMatrix.Contains(gridPosition))
            _collisionsMatrix.Remove(gridPosition);
    }

    private void CreateItemInRandomPlace(GridObject template)
    {
        // var randomGridX = Random.Range()
    } 
}
