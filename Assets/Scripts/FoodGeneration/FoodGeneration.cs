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
    [SerializeField] private int _cellCount = 5;
    [SerializeField] private Transform _leftDownPoint;
    [SerializeField] private Transform _rightUpPoint;
    [SerializeField] private Transform _ground;

    private List<Vector3Int> _collisionsMatrix = new List<Vector3Int>();
    private float _generationHeight;
    private float _delayCreating;
    private Queue<Vector3Int> _foodDestroying;
    private Vector3 _startPosition;
    private Vector3 _endPosition;
    
    private void Awake()
    {

        _startPosition = _leftDownPoint.position;
        _endPosition = _rightUpPoint.position;
        _cellSize = (_endPosition.x - _startPosition.x) / _cellCount; //to do replace 5 by cell count from inspector
        // Debug.Log("BBB cellSize: " + _cellSize);
        // Debug.Log("BBB start position world: " + _startPosition);
        // Debug.Log("BBB start position grid: " + WorldToGridPosition(_startPosition));
        // Debug.Log("BBB end position world: " +_endPosition);
        // Debug.Log("BBB end position grid: " + WorldToGridPosition(_endPosition));
        
        _foodDestroying = new Queue<Vector3Int>();
        _generationHeight = _ground.position.y + 0.7f + 2.56f;
        FillArea(_startPosition, _endPosition, 1);
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
            var randomZ = Random.Range(startPoint.z, endPoint.z);
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
    
        for (int x = startPoint.x ; x < _cellCount ; x++)
        {
            for (int z = startPoint.z ; z < _cellCount ; z++)
            {
                // Debug.Log("BBB x=" + x + "; z=" + z);
                TryCreate(_foodPrefabs[0],new Vector3Int(x, 0, z));
            }
        }
    }

    private bool TryCreate(GridObject template, Vector3Int gridPosition)
    {
        if (_collisionsMatrix.Contains(gridPosition))
            return false;
        else
            _collisionsMatrix.Add(gridPosition);
        
        if (template == null)
            return false;

        if (template.Chance > Random.Range(0, 100))
        {
            CreateFood(template, gridPosition, false);
        }
        return true;
    }

    
    private Vector3 GridToWorldPosition(Vector3Int gridPosition)
    {
        return new Vector3(
            gridPosition.x * _cellSize + _startPosition.x,
            0,
            gridPosition.z * _cellSize + _startPosition.z);
    }
    
    private Vector3Int WorldToGridPosition(Vector3 worldPosition)
    {
        return new Vector3Int(
            (int)((worldPosition.x - _startPosition.x) / _cellSize),
            0,
            (int)((worldPosition.z - _startPosition.z) / _cellSize));
    }

    private void OnDied(Vector3Int gridPosition)
    {
        // Debug.Log("AAA-3 DIED !");
        TryRemoveFromCollisionsMatrix(gridPosition);
        _foodDestroying.Enqueue(gridPosition);
        
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
            CreateFood(_primaryFoodPrefabs[0], _foodDestroying.Dequeue(), true);
        else
            CreateFood(_foodPrefabs[0], _foodDestroying.Dequeue(), true);
    }

    private void CreateFood(GridObject template, Vector3Int gridPosition, bool isSmoothShowing)
    {
        int rotateY = Random.Range(0, 180);
        Quaternion quaternionY = Quaternion.Euler(0, rotateY, 0);

        Vector3 position = GridToWorldPosition(gridPosition) + GetRandomOffset();
        // Vector3 position = isSmoothShowing
        //     ? GridToWorldPosition(gridPosition) + GetRandomOffset()
        //     : GridToWorldPosition(gridPosition);
        
        var gridObject = Instantiate(template, position + new Vector3(0, _generationHeight, 0), quaternionY, transform);
        
        Vector3 scale = gridObject.transform.localScale;
        gridObject.Died += OnDied;
        gridObject.SetCoordinads(gridPosition);
        if (isSmoothShowing)
        {
            gridObject.transform.DOScale(0, 0);
            gridObject.transform.DOScale(scale, 0.5f);           
        }
    }

    private Vector3 GetRandomOffset()
    {
        float offsetX = Random.Range(0.5f, _cellSize - 0.3f);
        float offsetZ = Random.Range(0.5f, _cellSize - 0.3f);
        // float offsetX = Random.Range(-0.15f, 0.15f);
        // float offsetZ = Random.Range(-0.15f, 0.15f);

        // return new Vector3(offsetX + 0.25f, 0, offsetZ + 0.45f); //cellSize = 1.25f
        return new Vector3(offsetX, 0, offsetZ);
    }
}
