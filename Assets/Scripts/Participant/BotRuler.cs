using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(ParticipantMover))]
public class BotRuler : MonoBehaviour
{
    private ParticipantMover _participantMover;
    private FoodGeneration _foodGeneration;
    private Food[] _foods;
    private Vector2 _direction;
    private Vector2 _startPosition;
    private bool _isDirectionChosen;
    private float _repeatTime;
    private Food _nearestFood;
    private Transform _targetFood;
    private Transform _target;
    private Quaternion _lookRotation;
    private Animator _animator;
    private Vector3 _moveDirection;

    private void Start()
    {
        _moveDirection = Vector3.zero;
        _participantMover = GetComponent<ParticipantMover>();
        _foodGeneration = FindObjectOfType<FoodGeneration>();
        _repeatTime = 0.75f;
        _animator = GetComponentInChildren<Animator>();
        _animator.SetFloat("Speed", _participantMover.Speed);
        // InvokeRepeating(nameof(SetRandomDirection), 0.5f, _repeatTime);
        // InvokeRepeating(nameof(GoToFood), 0.5f, _repeatTime);
        GetComponent<Participant>().FoodEatenByBot += OnFoodEaten;
        _targetFood = GetNearestFood().transform;

        InvokeRepeating(nameof(Rotate), 0, 0.05f);
    }

    private void Update()
    {
        if (_targetFood != null)
        {
            _moveDirection = (_targetFood.position - transform.position).normalized;
            transform.Translate(Time.deltaTime * _participantMover.Speed * _moveDirection, Space.World);
        }
        else
        {
            _targetFood = GetNearestFood();
        }
    }

    private void OnFoodEaten(Food food)
    {
        // Debug.Log("AAA Food Eaten!!! _foods: " + _foods);
        // Debug.Log("AAA Food Eaten!!! index : " + Array.IndexOf(_foods, food));
        List<Food> temp = _foods.ToList();
        temp.Clear();
        _foods = temp.ToArray();
        // Debug.Log("AAA Food Eaten!!! _foods: " + _foods);
        
        // if (IsRopeNextTo(out TouchBorder touchBorder) == false)
        Invoke(nameof(SetNewTarget), Time.deltaTime);
        // else
        // {
        //     // _participantMover.DoRopeRepulsion(_moveDirection, touchBorder);
        //     // _participantMover.DoRopeRepulsion(_moveDirection, );
        //     // MoveToRope(Vector3 direction);
        // }
    }


    private bool IsRopeNextTo(out TouchBorder touchBorder)
    {
        Vector3 newPosition = 1.8f * _moveDirection + transform.position;

        if (_participantMover.IsOutField(newPosition, out TouchBorder touchBorder2))
        {
            touchBorder = touchBorder2;
            // return true;
        }

        touchBorder = TouchBorder.NULL;
        return false;
    }

    private TouchBorder GetTouchBorder()
    {
        return TouchBorder.NULL;
    }

    private void SetNewTarget()
    {
        _targetFood = GetNearestFood();
    }

    private void Rotate()
    {
        _lookRotation = Quaternion.LookRotation(_targetFood.position - transform.position);
        transform.rotation = Quaternion.Lerp(transform.rotation, _lookRotation, 0.5f);
    }
    
    private Transform GetNearestFood()
    {
        _foods = _foodGeneration.gameObject.GetComponentsInChildren<Food>();
        // Debug.Log("AAA Count  of food : " + _foods.Length);
        float shortestDistance = Mathf.Infinity;
        _nearestFood = null;

        foreach (Food food in _foods)
        {
            float distanceToFood = Vector3.Distance(transform.position, food.gameObject.transform.position);
            if (distanceToFood < shortestDistance)
            {
                shortestDistance = distanceToFood;
                _nearestFood = food;
            }
        }
        return _nearestFood != null ? _nearestFood.transform : null;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        if (_targetFood != null) Gizmos.DrawWireSphere(_targetFood.transform.position, 0.5f);
    }
}