using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMover : MonoBehaviour
{
    [SerializeField] private Transform _leftDownPointBorder;
    [SerializeField] private Transform _rightUpPointBorder;
    [SerializeField] private float _offsetBorder;
    [SerializeField] private float _speed;
    [SerializeField] private float _kickbackForce = 1; 
    [SerializeField] private float _boost = 2; 
    
    private Vector3 _directionWorld;
    private Quaternion _lookRotation;
    private bool _isOppositeMoving;
    private Vector3 _oppositeDirection;
    private Coroutine _boostCoroutine;
    private float _startSpeed;
    private Rigidbody _rigidbody;
    private float _radiusCollider;
    private Vector3 _leftUpPointBorderPosition;
    private Vector3 _rightDownPointBorderPosition;

    private void Start()
    {
        _speed = 3f;
        _isOppositeMoving = false;
        _startSpeed = _speed;
        _rigidbody = GetComponent<Rigidbody>();
        _radiusCollider = GetComponent<CapsuleCollider>().radius;
        _leftUpPointBorderPosition = new Vector3(_leftDownPointBorder.position.x, _leftDownPointBorder.position.y,
            _rightUpPointBorder.position.z);
        _rightDownPointBorderPosition = new Vector3(_rightUpPointBorder.position.x, _leftDownPointBorder.position.y,
            _leftDownPointBorder.position.z);
        // _rigidbody.isKinematic = true;
    }

    private void Update()
    {
        if (_isOppositeMoving)
        {
            Move(_oppositeDirection);
            if (GetExcessDirection().magnitude < 0.01f)
                _isOppositeMoving = false;
        }
    }

    public void TryMove(Vector2 direction)
    {
        var worldDirection = GetWorldDirection(direction);

        if (IsCrossedBorder() == false)// && IsNextToPilliar() == false)
        {
            Rotate(worldDirection);
            Move(worldDirection);
        }
        else
        {
            // if (IsNextToPilliar() == false)
            // {
                _oppositeDirection = GetExcessDirection() * 2;
                _isOppositeMoving = true;
                
                if (_speed == _startSpeed)
                    _boostCoroutine = StartCoroutine(StartBoost());                
            // }
            // else
            // {
            //     Debug.Log("AAA-4 Next to pilliar");
            // }
        }
    }

    private Vector3 GetWorldDirection(Vector2 direction)
    {
        return new Vector3(direction.x, transform.position.y, direction.y);
    }

    private void Move(Vector3 direction)
    {
        var offsetPosition = direction.normalized * Time.deltaTime * _speed;
        var newPosition = transform.position + new Vector3(offsetPosition.x, 0, offsetPosition.z);
        // transform.position = transform.position + new Vector3 (offsetPosition.x, 0, offsetPosition.z);   

        if (IsNextToPilliar(newPosition, 1.25f) == false)
            transform.position = newPosition;

        Debug.Log("GetExcessDirection transform.position : " + transform.position);

    }

    private void Rotate(Vector3 direction)
    {
        _lookRotation = Quaternion.LookRotation(direction);
        transform.rotation = _lookRotation;        
    }

    private bool IsCrossedBorder()
    {
        var currentPosition = transform.position;
        
        Debug.Log("AAA-3 currentPosition : " + currentPosition );
        Debug.Log("AAA-3 _leftDownPointBorder : " + _leftDownPointBorder );
        Debug.Log("AAA-3 _rightUpPointBorder : " + _rightUpPointBorder );

        // _rigidbody.isKinematic = true;
        //
        // if (currentPosition.x < _leftDownPointBorder.position.x + 1 &&
        //     currentPosition.z < _leftDownPointBorder.position.z + 1)
        //     _rigidbody.isKinematic = false;
        //
        // if (currentPosition.x < _leftDownPointBorder.position.x + 1 && currentPosition.z > _rightUpPointBorder.position.z - 1)
        //     _rigidbody.isKinematic = false;
        //
        // if (currentPosition.x > _rightUpPointBorder.position.x - 1 && currentPosition.z < _leftDownPointBorder.position.z + 1)
        //     _rigidbody.isKinematic = false;      
        //
        // if (currentPosition.x > _rightUpPointBorder.position.x - 1 && currentPosition.z > _rightUpPointBorder.position.z - 1)
        //     _rigidbody.isKinematic = false;
        
        
        if (currentPosition.x < _leftDownPointBorder.position.x - _offsetBorder)
            return true;      
        
        if (currentPosition.x > _rightUpPointBorder.position.x + _offsetBorder)
            return true;  
        
        if (currentPosition.z < _leftDownPointBorder.position.z - _offsetBorder)
            return true;  
        
        if (currentPosition.z > _rightUpPointBorder.position.z + _offsetBorder)
            return true;
        
        return false;
    }

    private bool IsNextToPilliar(Vector3 position, float offset)
    {
        if (Vector3.Distance(position, _leftDownPointBorder.position) < offset)
            return true;
        if (Vector3.Distance(position, _rightUpPointBorder.position) < offset)
            return true;
        if (Vector3.Distance(position, _leftUpPointBorderPosition) < offset)
            return true;
        if (Vector3.Distance(position, _rightDownPointBorderPosition) < offset)
            return true;
        
        return false;
    }

    private void SetPositionNextToPilliar()
    {
        // if (Vector3.Distance(transform.position, _leftDownPointBorder.position) < 1)
        // {
        //     transform.position = new Vector3(transform.position.x, transform.position.y, )
        // }
    }

    private bool IsCrossedTwoBorders()
    {
        var currentPosition = transform.position;
        
        
        
        // if (currentPosition.x < _leftDownPointBorder.position.x - _offsetBorder && currentPosition.z < _leftDownPointBorder.position.z - _offsetBorder)
        //     return true;      
        
        // if (currentPosition.x > _rightUpPointBorder.position.x + _offsetBorder)
        //     return true;  
        //
        // if (currentPosition.z < _leftDownPointBorder.position.z - _offsetBorder)
        //     return true;  
        //
        // if (currentPosition.z > _rightUpPointBorder.position.z + _offsetBorder)
        //     return true;
        
        return false;
    }

    private Vector3 GetExcessDirection()
    {
        var currentPositionX = transform.position.x;
        var currentPositionZ = transform.position.z;

        if (currentPositionX < _leftDownPointBorder.position.x - _offsetBorder + _kickbackForce)
            return new Vector3(_leftDownPointBorder.position.x - currentPositionX + _kickbackForce, 0 ,0);
        
        if (currentPositionX > _rightUpPointBorder.position.x + _offsetBorder - _kickbackForce)
            return new Vector3(currentPositionX - _rightUpPointBorder.position.x - _kickbackForce, 0, 0);     
        
        if (currentPositionZ < _leftDownPointBorder.position.z - _offsetBorder + _kickbackForce)
            return new Vector3(0, 0, _leftDownPointBorder.position.z - currentPositionZ + _kickbackForce);  
        
        if (currentPositionZ > _rightUpPointBorder.position.z + _offsetBorder - _kickbackForce)
            return new Vector3(0, 0, currentPositionZ - _rightUpPointBorder.position.z - _kickbackForce);

        return Vector3.zero;
    }

    private IEnumerator StartBoost()
    {
        _speed *= _boost;
        yield return new WaitForSeconds(1);
        _speed = _startSpeed;
        StopCoroutine(_boostCoroutine);
    }

    private void OnTriggerStay(Collider other)
    {
        throw new NotImplementedException();
    }
}
