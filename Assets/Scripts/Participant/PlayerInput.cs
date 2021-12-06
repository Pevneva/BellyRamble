using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(ParticipantMover))]
public class PlayerInput : MonoBehaviour
{
    private ParticipantMover _participantMover;
    private Vector2 _direction;
    private Vector2 _startPosition;
    private bool _isDirectionChosen;

    private void Start()
    {
        _participantMover = GetComponent<ParticipantMover>();
    }

    private void FixedUpdate()
    {
        TrySetDirection();

        if (_isDirectionChosen)
        {
            _participantMover.TryMove(_direction);
        }
    }

    private void Update()
    {
        TrySaveStartData();
        TryResetStartData();
    }

    private void TrySaveStartData()
    {
        if (Input.GetMouseButtonDown(0))
        {
            _startPosition = Input.mousePosition;
            _isDirectionChosen = false;
        }
    }
    
    private void TryResetStartData()
    {
        if (Input.GetMouseButtonUp(0))
        {
            _isDirectionChosen = false;
            _participantMover.StopMoving();            
        }
    }

    private void TrySetDirection()
    {
        if (Input.GetMouseButton(0))
        {
            Vector2 currentMousePosition = Input.mousePosition;
            
            if (Vector2.Distance(_startPosition, currentMousePosition) > 5f) //to do radius variable instead of number
            {
                _direction = currentMousePosition - _startPosition;
                _isDirectionChosen = true;
            }            
        }
    }
}
