using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;
using DG.Tweening;

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
    private Queue<Vector3Int> _foodDestroying;
    
    private void Start()
    {
        _foodDestroying = new Queue<Vector3Int>();
        _generationHeight = _ground.position.y + 0.2f;
        FillArea(_leftDownPoint.position, _rightUpPoint.position, 1);
        
        var distanceX = Vector3.Distance(_leftDownPoint.position,
            new Vector3(_rightUpPoint.position.x, _leftDownPoint.position.y, _leftDownPoint.position.z));
    }

    private void Update()
    {
        // FillArea(_leftDownPoint.position, _rightUpPoint.position, 1);
    }

    private void CreateFoodRandomPosition(Vector3 leftDownPoint, Vector3 rightUpPoint)
    {
        var startPoint = WorldToGridPosition(leftDownPoint);
        var endPoint = WorldToGridPosition(rightUpPoint);

        bool isFoodCreated = false;
        int counter = 0;
        while (isFoodCreated == false && counter++  < 10000)
        {
            var randomX = Random.Range(startPoint.x, endPoint.x);
            var randomZ = Random.Range(startPoint.z + 1, endPoint.z);
            Debug.Log("AAA-4 counter : " + counter);
            isFoodCreated = TryCreate(_foodPrefabs[0],new Vector3Int(randomX, 0, randomZ));
        }
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
                // TryCreate(_primaryFoodPrefabs[0], new Vector3Int(x, 0, z));
            }
        }
    }

    private void AddFood()
    {
        
    }
    
    private bool TryCreate(GridObject template, Vector3Int gridPosition)
    {
        if (_collisionsMatrix.Contains(gridPosition))
            return false;
        else
            _collisionsMatrix.Add(gridPosition);
        
        // var template = GetRandomTemplate();
        
        if (template == null)
            return false;
        
        
        
        // Vector3 position = GridToWorldPosition(gridPosition);
        // float offsetX = Random.Range(-0.3f, 0.3f);
        // float offsetZ = Random.Range(-0.3f, 0.3f);
        // var rotateY = Random.Range(0, 180);
        // Quaternion quaternionY = Quaternion.Euler(0, rotateY, 0);
        //
        // position = position + new Vector3(offsetX + 0.25f, -1, offsetZ + 0.35f);

        if (template.Chance > Random.Range(0, 100))
        {
            CreateFood(template, gridPosition, true);
            // var gridObject = Instantiate(template, position, quaternionY, transform);
            // gridObject.Died += OnDied;
            // gridObject.SetCoordinads(gridPosition);
        }

        return true;
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
        Debug.Log("AAA-3 DIED !");
        TryRemoveFromCollisionsMatrix(gridPosition);
        _foodDestroying.Enqueue(gridPosition);
        // CreateFoodRandomPosition(_leftDownPoint.position, _rightUpPoint.position);
        if (_foodDestroying.Count > 5)
            CreateItemFromFoodQueue();
    }

    public void TryRemoveFromCollisionsMatrix(Vector3Int gridPosition)
    {
        if (_collisionsMatrix.Contains(gridPosition))
            _collisionsMatrix.Remove(gridPosition);
    }

    private void CreateItemFromFoodQueue()
    {
        int randomValue = Random.Range(0, 10);
        if (randomValue == 0 )
            CreateFood(_primaryFoodPrefabs[0], _foodDestroying.Dequeue(), false);
        else
            CreateFood(_foodPrefabs[0], _foodDestroying.Dequeue(), false);
        // Vector3Int gridPosition = _foodDestroying.Dequeue();
        // Vector3 position = GridToWorldPosition(gridPosition) + Vector3.down;
        
        // int rotateY = Random.Range(0, 180);
        // Quaternion quaternionY = Quaternion.Euler(0, rotateY, 0);
        // var gridObject = Instantiate(template, position, quaternionY, transform);
        // gridObject.Died += OnDied;
        // gridObject.SetCoordinads(gridPosition);
    }

    private void CreateFood(GridObject template, Vector3Int gridPosition, bool isOffsetAdded)
    {
        int rotateY = Random.Range(0, 180);
        Quaternion quaternionY = Quaternion.Euler(0, rotateY, 0);

        Vector3 position = isOffsetAdded
            ? GridToWorldPosition(gridPosition) + GetRandomOffset()
            : GridToWorldPosition(gridPosition);
        var gridObject = Instantiate(template, position + Vector3.down, quaternionY, transform);
        Vector3 scale = gridObject.transform.localScale;
        gridObject.Died += OnDied;
        gridObject.SetCoordinads(gridPosition);
        if (isOffsetAdded == false)
        {
            gridObject.transform.DOScale(0, 0);
            gridObject.transform.DOScale(scale, 0.5f);           
        }
    }

    private Vector3 GetRandomOffset()
    {
        // float offsetX = Random.Range(-0.3f, 0.3f);
        // float offsetZ = Random.Range(-0.3f, 0.3f);
        float offsetX = Random.Range(-0.15f, 0.15f);
        float offsetZ = Random.Range(-0.15f, 0.15f);

        return new Vector3(offsetX + 0.25f, 0, offsetZ + 0.45f);
    }
}
