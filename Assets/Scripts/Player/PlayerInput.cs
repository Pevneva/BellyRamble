using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(PlayerMover))]
public class PlayerInput : MonoBehaviour
{
    [SerializeField] private float _speed = 1;

    private PlayerMover _playerMover;
    private Vector2 _currentMousePosition;
    private Vector2 _currentPlayerPosition;
    private Vector2 _direction;
    private Vector3 _directionWorld;
    private Quaternion _lookRotation;
    private Vector2 _startPosition;
    private bool _isDirectionChosen;
    
    public event UnityAction<Vector2> SwipeDone;
    
    private void Start()
    {
        _playerMover = GetComponent<PlayerMover>();
    }

    private void Update()
    {
        TryGetDirection();
    }
    
    private void TryGetDirection()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("AAA-2-1 START !!! ");
            _startPosition = Input.mousePosition;
            _isDirectionChosen = false;
        } 
        else if (Input.GetMouseButton(0))
        {
            Vector2 currentMousePosition = Input.mousePosition;
            
            if (Vector2.Distance(_startPosition, currentMousePosition) > 5f) //to do radius variable instead of number
            {
                _direction = currentMousePosition - _startPosition;
                _isDirectionChosen = true;
            }

        } 
        else if (Input.GetMouseButtonUp(0))
        {
            _isDirectionChosen = false;
        }  
        
        if (_isDirectionChosen)
            _playerMover.TryMove(_direction);
    }
    
    // void TestMove2(){ AAA
    //     float rotX = Input.GetAxis("Mouse X") * 1 * Mathf.Deg2Rad;
    //     float rotY = Input.GetAxis("Mouse Y") * 1 * Mathf.Deg2Rad;
    //
    //
    //     transform.RotateAround(Vector3.up, -rotX);
    //     transform.RotateAround(Vector3.right, rotY);
    //     
    //     // _currentMousePosition = Input.mousePosition;
    //     // _currentPlayerPosition = Camera.main.WorldToScreenPoint(transform.position);
    //     //  // _direction = _currentMousePosition - _currentPlayerPosition;
    //     // _direction = _currentPlayerPosition - _currentMousePosition;
    //     // _directionWorld = new Vector3(_direction.x, transform.position.y, _direction.y);
    //     // _lookRotation = Quaternion.LookRotation(_directionWorld);
    //     // transform.rotation = _lookRotation;
    //     //
    //     // transform.position = Vector3.MoveTowards(transform.position, -_directionWorld.normalized * _speed * 2, 0.1f);
    //     // // transform.position = transform.position - _directionWorld.normalized * Time.deltaTime * _speed * 50;
    //     // Debug.Log("=============================== ");
    //     // Debug.Log("_currentMousePosition : " + _currentMousePosition);
    //     // Debug.Log("_currentPlayerPosition : " + _currentPlayerPosition);
    //     // Debug.Log("_direction : " + _direction);
    // }
    //
    // private void MoveTest()
    // {
    //     if (Input.touchCount > 0)
    //     {
    //         Touch touch = Input.GetTouch(0);
    //     
    //         // Handle finger movements based on touch phase.
    //         switch (touch.phase)
    //         {
    //             // Record initial touch position.
    //             case TouchPhase.Began:
    //                 Debug.Log("INPUT Began : ");
    //                 _startPosition = touch.position;
    //                 _isDirectionChosen = false;
    //                 break;
    //     
    //             // Determine direction by comparing the current touch position with the initial one.
    //             case TouchPhase.Moved:
    //                 // _direction = touch.position - _startPos;
    //                 // Debug.Log("INPUT _direction : " + _direction);
    //                 break;
    //     
    //             // Report that a direction has been chosen when the finger is lifted.
    //             case TouchPhase.Ended:
    //                 _isDirectionChosen = true;
    //                 
    //                 Vector2 mousePoint = Input.mousePosition;
    //                 _direction = mousePoint - _startPosition;
    //                 
    //                 Debug.Log("INPUT Ended : ");
    //                 Debug.Log("INPUT Ended _startPos : " + _startPosition);
    //                 Debug.Log("INPUT Ended : mousePoint : " + mousePoint);
    //                 Debug.Log("INPUT Ended : distance : " + Vector2.Distance(_startPosition, mousePoint));
    //     
    //                 if (Vector2.Distance(_startPosition, mousePoint)>5f)
    //                     SwipeDone?.Invoke(_direction.normalized * 10);
    //                 break;
    //         }
    //     }
    //     if (_isDirectionChosen)
    //     {
    //         // Something that uses the chosen direction...
    //         
    //     }   
    // } 
    //
    // private void TryMove1()
    // {
    //     
    // }

}
