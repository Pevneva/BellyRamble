using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
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
    private Transform _target;
    // private Transform _target;
    private Quaternion _lookRotation;
    private Animator _animator;
    private Vector3 _moveDirection;
    private Vector3 _newPosition;
    private TouchBorder _touchBorder;
    private Player _player;
    private bool _isRepulsion;
    private bool _isFlying;
    private Participant[] _participants;

    private void Start()
    {
        _isRepulsion = false;
        _isFlying = false;
        _newPosition = new Vector3(Mathf.Infinity, Mathf.Infinity);
        _moveDirection = Vector3.zero;
        _player = FindObjectOfType<Player>();
        _participantMover = GetComponent<ParticipantMover>();
        _foodGeneration = FindObjectOfType<FoodGeneration>();
        _repeatTime = 0.75f;
        _animator = GetComponentInChildren<Animator>();
        _animator.SetFloat("Speed", _participantMover.Speed);
        // InvokeRepeating(nameof(SetRandomDirection), 0.5f, _repeatTime);
        // InvokeRepeating(nameof(GoToFood), 0.5f, _repeatTime);
        GetComponent<Participant>().FoodEatenByBot += OnFoodEaten;
        _target = GetNearestFood().transform;

        _participants = FindObjectsOfType<Participant>();
        foreach (var VARIABLE in _participants)
        {
            Debug.Log("AAA PARTICIPENT: " + VARIABLE);
            // if (VARIABLE is GameObject.getC)
            // {
            //     
            // }
        }
        InvokeRepeating(nameof(Rotate), 0, 0.1f);
    }

    private void Update()
    {
        if (Vector3.Distance(transform.position, _newPosition) < 0.5f)
        {
            _newPosition = new Vector3(Mathf.Infinity, Mathf.Infinity);
            Debug.Log("SAS DoRopeRepulsion _moveDirection : " + _moveDirection);
            Debug.Log("SAS DoRopeRepulsion _touchBorder : " + _touchBorder);
            Debug.Log("SASA RepulsionTime : " + _participantMover.RepulsionTime);
            Debug.Log("SASA BoostTime : " + _participantMover.BoostTime);
            _participantMover.DoRopeRepulsion(_moveDirection * 100, _touchBorder, true);
            _target.position = _participantMover.NewPosition;
            _isRepulsion = true;
            Invoke(nameof(SetParticipantDirection), _participantMover.RepulsionTime);
            Invoke(nameof(SetNewTarget), _participantMover.RepulsionTime + _participantMover.BoostTime);
        }
        
        if (_isRepulsion)
            return;

        if (_isFlying)
            return;
        
        if (_target != null)
        {
            _moveDirection = (_target.position - transform.position).normalized;
            transform.Translate(Time.deltaTime * _participantMover.Speed * _moveDirection, Space.World);
        }
        else
        {
            _target = GetNearestFood();
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

        if (IsRopeNextTo(out TouchBorder touchBorder) == false)
        {
            // Debug.Log("SAS SetNewTarget !!!");
            Invoke(nameof(SetNewTarget), Time.deltaTime);
        }
        else
        {
            Debug.Log("SAS ROPE !!!");
            Transform targetTransform = new GameObject().transform;
            targetTransform.position = _newPosition;
            _target = targetTransform;
            
            // Debug.Log("SAS _target : " + _target);

            _touchBorder = touchBorder;
            // _participantMover.DoRopeRepulsion(_moveDirection, touchBorder);
            // _participantMover.DoRopeRepulsion(_moveDirection, );
            // MoveToRope(Vector3 direction);
        }
    }
    
    private bool IsRopeNextTo(out TouchBorder touchBorder)
    {
        
        // Vector3 newPosition = 1.5f * _moveDirection.normalized + transform.position;
        Vector3 newPosition = 1.5f * _moveDirection.normalized + transform.position;
        if (_participantMover.IsOutField(newPosition, out TouchBorder touchBorder2))
        {
            Debug.Log("SASA Rope is next to me ! Border = " + touchBorder2);

            _newPosition = newPosition;
            touchBorder = touchBorder2;
            return true;
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
        Debug.Log("SASA SetNewTarget");
        _target = GetNearestFood();
        // _animator.SetFloat("Speed", 0);
    }

    private void SetParticipantDirection()
    {
        // Participant participant;
        // participant = FindObjectOfType<Participant>();
        // _target = participant.gameObject.transform;
        _target = _player.gameObject.transform;
        Debug.Log("SASA SetParticipantDirection _target : " + _target);
        _isRepulsion = false;
    }

    private void Rotate()
    {
        if (_target == null)
            return;

        if (_isRepulsion)
            return;
                
        _lookRotation = Quaternion.LookRotation(_target.position - transform.position);
        transform.rotation = Quaternion.Lerp(transform.rotation, _lookRotation, 0.5f);
    }
    
    private Transform GetNearestFood()
    {
        _foods = _foodGeneration.gameObject.GetComponentsInChildren<Food>();
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
        if (_target != null) Gizmos.DrawWireSphere(_target.transform.position, 0.5f);
    }
}