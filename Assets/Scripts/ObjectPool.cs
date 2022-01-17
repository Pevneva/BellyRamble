using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

public class ObjectPool : MonoBehaviour
{
    private List<GameObject> _pool = new List<GameObject>();
    private List<Image> _imagePool = new List<Image>();

    public void Initialize(GameObject prefab, GameObject _contaner, int _capacity)
    {
        for (int i = 0; i < _capacity; i++)
        {
            var objectPool = Instantiate(prefab, _contaner.transform);
            objectPool.SetActive(false);

            _pool.Add(objectPool);
        }
    }
    
    public void Initialize(Image prefab, GameObject _contaner, int _capacity)
    {
        for (int i = 0; i < _capacity; i++)
        {
            var objectPool = Instantiate(prefab, _contaner.transform);
            objectPool.gameObject.SetActive(false);

            _imagePool.Add(objectPool);
        }
    }

    protected void Initialize(GameObject[] prefabs, GameObject _contaner, int _capacity)
    {
        for (int i = 0; i < _capacity; i++)
        {
            var randomNumberPrefab = Random.Range(0, prefabs.Length);
            var objectPool = Instantiate(prefabs[randomNumberPrefab], _contaner.transform);
            objectPool.SetActive(false);

            _pool.Add(objectPool);
        }
    }

    public bool TryGetObject(out GameObject result)
    {
        result = _pool.FirstOrDefault(t => t.activeSelf == false);
        return result != null;
    }
    
    public bool TryGetObject(out Image result)
    {
        result = _imagePool.FirstOrDefault(t => t.gameObject.activeSelf == false);
        Debug.Log("EEE result image : " + result);
        return result != null;
    }
}
