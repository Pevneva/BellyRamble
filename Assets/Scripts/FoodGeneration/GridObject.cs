using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GridObject : MonoBehaviour
{
    [SerializeField] private int _chance;

    public event UnityAction<Vector3Int> Died;
    public Vector3Int Coordinads { get; private set; }

    public int Chance => _chance;

    private void OnValidate()
    {
        _chance = Mathf.Clamp(_chance, 1, 100);
    }
    
    
    private void OnDestroy()
    {
        Died?.Invoke(Coordinads);
    }

    public void SetCoordinads(Vector3Int coordinads)
    {
        Coordinads = coordinads;
    }
}
