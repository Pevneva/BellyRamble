using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodUtils : MonoBehaviour
{
    private FoodGeneration _foodGeneration;
    private Food[] _foods;
    
    private void Awake()
    {
        _foodGeneration = FindObjectOfType<FoodGeneration>();
    }
    
    public Transform GetNearestFood(Transform target)
    {
        Food nearestFood = null;
        _foods = _foodGeneration.gameObject.GetComponentsInChildren<Food>();
        float shortestDistance = Mathf.Infinity;
        
        foreach (Food food in _foods)
        {
            float distanceToFood = Vector3.Distance(target.position, food.gameObject.transform.position);
            if (distanceToFood < shortestDistance)
            {
                shortestDistance = distanceToFood;
                nearestFood = food;
            }
        }

        return nearestFood != null ? nearestFood.transform : null;
    }
}
