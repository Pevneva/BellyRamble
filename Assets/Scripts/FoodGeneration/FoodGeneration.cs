using System.Collections.Generic;
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
        _cellSize = (_endPosition.x - _startPosition.x) / _cellCount; 
        _foodDestroying = new Queue<Vector3Int>();
        // _generationHeight = _ground.position.y + 0.7f + 2.56f;
        _generationHeight = _ground.position.y + 0.35f + 2.56f;
        FillArea(_startPosition);
    }
    
    private void FillArea(Vector3 leftDownPoint)
    {
        var startPoint = WorldToGridPosition(leftDownPoint);

        for (int x = startPoint.x ; x < _cellCount ; x++)
        {
            for (int z = startPoint.z ; z < _cellCount ; z++)
            {
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
        TryRemoveFromCollisionsMatrix(gridPosition);
        _foodDestroying.Enqueue(gridPosition);
        
        if (_foodDestroying.Count > 5)
            CreateItemFromFoodQueue();
    }

    private void TryRemoveFromCollisionsMatrix(Vector3Int gridPosition)
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
        return new Vector3(offsetX, 0, offsetZ);
    }
}