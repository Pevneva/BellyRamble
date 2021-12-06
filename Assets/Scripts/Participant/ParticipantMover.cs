using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[RequireComponent(typeof(Rigidbody))]
public class ParticipantMover : MonoBehaviour
{
    [SerializeField] private Transform _leftDownPointBorder;
    [SerializeField] private Transform _rightUpPointBorder;
    [SerializeField] private float _speed;
    [SerializeField] private float _boost = 2;

    private bool _isForcing;
    private float _positionY;
    private float _startSpeed;
    private Rigidbody _rigidbody;
    private Participant _participant;
    private Coroutine _boostCoroutine;
    private Animator _animator;
    private Quaternion _lookRotation;

    private void Start()
    {
        _isForcing = false;
        _positionY = transform.position.y;
        _startSpeed = _speed;
        _rigidbody = GetComponent<Rigidbody>();
        _participant = GetComponent<Participant>();
        _animator = GetComponentInChildren<Animator>();
        _animator.SetFloat("Speed", 0f);

        _participant.RopeTouchStarted += OnRopeTouchStarted;
        _participant.RopeTouchEnded += OnRopeTouchEnded;
        _participant.BotTouched += AddForceFly;
    }

    public void TryMove(Vector2 direction)
    {
        Rotate(direction);
        Move(direction);
    }

    private void Rotate(Vector2 direction)
    {
        _lookRotation = Quaternion.LookRotation(GetWorldDirection(direction));
        transform.rotation = _lookRotation;
    }

    private Vector3 GetWorldDirection(Vector2 direction)
    {
        return new Vector3(direction.x, _positionY, direction.y);
    }

    private void Move(Vector2 direction)
    {
        if (_isForcing)
            return;

        var moveDirection = new Vector3(direction.x, 0, direction.y);
        _animator.SetFloat("Speed", moveDirection.normalized.magnitude * _speed);
        transform.position += Time.deltaTime * _speed * 2 * moveDirection.normalized;
    }

    public void StopMoving()
    {
        if (_participant.IsRopeTouching)
        {
            _rigidbody.isKinematic = false;
            Debug.Log("AAA-135 DIR: " + GetDiscardingDirection());
            AddForce(GetDiscardingDirection());
        }
        else
        {
            Debug.Log("AAA-13 IsRopeTouching = false");
            _rigidbody.velocity = Vector3.zero;
            _animator.SetFloat("Speed", 0);
        }
    }

    private void OnRopeTouchStarted()
    {
        Debug.Log("AAA-13 STARTED");
        _rigidbody.isKinematic = false;
        _participant.RopeTouchStarted -= OnRopeTouchStarted;
    }

    private void OnRopeTouchEnded()
    {
        if (_startSpeed == _speed)
            StartCoroutine(StartBoost());

        _participant.RopeTouchEnded -= OnRopeTouchEnded;
    }

    private Vector3 GetDiscardingDirection()
    {
        float positionX = transform.position.x;
        float positionZ = transform.position.z;

        Debug.Log("AAA-135 DIR positionX : " + positionX);
        Debug.Log("AAA-135 DIR positionZ : " + positionZ);

        if (positionX < _leftDownPointBorder.position.x + 0.5f)
            return Vector3.right;
        if (positionX > _rightUpPointBorder.position.x - 0.5f)
            return Vector3.left;
        if (positionZ < _leftDownPointBorder.position.z + 0.5f)
            return Vector3.forward;
        if (positionZ > _rightUpPointBorder.position.z - 0.5f)
            return Vector3.back;

        return Vector3.zero;
    }

    private void AddForce(Vector3 direction)
    {
        if (_isForcing)
            return;

        Debug.Log("AAA-135 FORCING ADDED " + Time.time);
        _isForcing = true;
        Invoke(nameof(SetIsKinematic), 0.2f);
        _rigidbody.AddForce(direction * 250, ForceMode.Impulse);
    }
    
    private void SetIsKinematic()
    {
        _rigidbody.isKinematic = true;
        _animator.SetFloat("Speed", 0);
        _participant.RopeTouchStarted += OnRopeTouchStarted;
        _isForcing = false;
    }
    
    private IEnumerator StartBoost()
    {
        _speed *= _boost;
        yield return new WaitForSeconds(1.5f);
        _speed = _startSpeed;
        _participant.RopeTouchEnded += OnRopeTouchEnded;
    }

    private void AddForceFly()
    {
        _rigidbody.constraints = RigidbodyConstraints.None;
        // _rigidbody.constraints == RigidbodyConstraints.FreezePosition 
        AddForce(new Vector3(1, 25, 1));
        _rigidbody.isKinematic = false;
    }

    // private IEnumerator StartAddForce(Vector3 direction)
    // {
    //     yield return new WaitForSeconds(0.5f);
    //     // AddForce(direction);
    //     yield break;
    // }
}