using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

[RequireComponent(typeof(ParticipantMover))]
public class BotRuler : MonoBehaviour
{
    private ParticipantMover _participantMover;
    private Vector2 _direction;
    private Vector2 _startPosition;
    private bool _isDirectionChosen;

    private void Start()
    {
        _participantMover = GetComponent<ParticipantMover>();
        
        InvokeRepeating(nameof(SetRandomDirection), 0.5f, 0.75f);
    }

    private void SetRandomDirection()
    {
        float randomX = Random.Range(-1.0001f, 1.001f);
        float randomY = Random.Range(-1.0001f, 1.001f);

        _direction = new Vector2(randomX, randomY);
        _isDirectionChosen = true;
        
        Debug.Log("AAA-136 _direction : " + _direction);
    }

    private void FixedUpdate()
    {
        if (_isDirectionChosen)
        {
            _participantMover.TryMove(_direction * 100);
            // _isDirectionChosen = false;
        }
    }


    // private void FixedUpdate()
    // {
    //     TrySetDirection();
    //
    //     if (_isDirectionChosen)
    //         _participantMover.TryMove(_direction);
    // }
    //
    // private void Update()
    // {
    //     TrySaveStartData();
    //     TryResetStartData();
    // }
    //
    // private void TrySaveStartData()
    // {
    //     if (Input.GetMouseButtonDown(0))
    //     {
    //         _startPosition = Input.mousePosition;
    //         _isDirectionChosen = false;
    //     }
    // }
    //
    // private void TryResetStartData()
    // {
    //     if (Input.GetMouseButtonUp(0))
    //     {
    //         _isDirectionChosen = false;
    //         _participantMover.StopMoving();            
    //     }
    // }
    //
    // private void TrySetDirection()
    // {
    //     if (Input.GetMouseButton(0))
    //     {
    //         Vector2 currentMousePosition = Input.mousePosition;
    //         
    //         if (Vector2.Distance(_startPosition, currentMousePosition) > 5f) //to do radius variable instead of number
    //         {
    //             _direction = currentMousePosition - _startPosition;
    //             _isDirectionChosen = true;
    //         }            
    //     }
    // }
}
