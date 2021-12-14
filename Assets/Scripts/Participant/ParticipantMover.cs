using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.Serialization;

[RequireComponent(typeof(Rigidbody))]
public class ParticipantMover : MonoBehaviour
{
    [SerializeField] private Transform _leftDownPointBorder;
    [SerializeField] private Transform _rightUpPointBorder;
    [SerializeField] private float _speed;
    [SerializeField] private float _boost;
    [SerializeField] private float _boostTime = 0.65f;

    public bool IsMoving { get; private set; }
    public bool IsPushing { get; private set; }


    private bool _isForcing;
    private bool _isRuling;
    private bool _isDelayingPushing;
    private float _positionY;
    private float _startSpeed;
    private float _delayCounter;
    private Rigidbody _rigidbody;
    private Participant _participant;
    private Coroutine _boostCoroutine;
    private Animator _animator;
    private FXContainer _fxContainer;
    private Quaternion _lookRotation;
    private Vector3 _startPosition;
    private float _turnOverTime;
    private float _preparingPushTime;
    private bool _isBoosting;
    private float _offsetToPush;

    private float _delayTime;
    private float _pushDistanceKoef;
    private float _pushTime;
    private bool _isTouchBreak;
    private Vector3 _discardingDirection;
    private float _angleRotateBeforePushing;
    private Vector3 _moveDirection;
    private float _angleRotation;

    private bool _isLeftBorder;
    private bool _isRightBorder;
    private bool _isUpBorder;
    private bool _isDownBorder;
    private float _radius;
    private float _offset;
    private Vector2 _planeStartPoint;
    private Vector2 _centerPositionXZ;
    private float _rotateStep;
    private float _rotateCounter;
    private float _movingCounter;
    private Vector3 _newPosition;
    private float _pushingDistance;

    private Sequence _turnOverSequence;
    // private Vector3 _newPosition;

    private void Start()
    {
        IsMoving = false;
        _isForcing = false;
        _isRuling = true;
        _isDelayingPushing = false;
        _rotateStep = 0.15f;
        _rotateCounter = 0;
        _offsetToPush = -0.1f;
        _delayCounter = 0;
        _turnOverTime = 0.2f;
        _preparingPushTime = 0.3f;
        _delayTime = 0.55f;
        _angleRotateBeforePushing = 15;
        _angleRotation = 180;
        // _pushDistanceKoef = 0.25f;
        // _pushTime = 0.025f;
        _pushDistanceKoef = 2f;
        _pushTime = 0.2f;
        _positionY = transform.position.y;
        _startSpeed = _speed;
        _rigidbody = GetComponent<Rigidbody>();
        _participant = GetComponent<Participant>();
        _animator = GetComponentInChildren<Animator>();
        _fxContainer = FindObjectOfType<FXContainer>();
        _fxContainer.StopParticipantEffects();
        _animator.SetFloat("Speed", 0f);
        _movingCounter = 0;
        _pushingDistance = 0;

        IsPushing = false;
        _isTouchBreak = false;
        _isBoosting = false;
        _offset = 0.5f;
        _centerPositionXZ = GetRingCenter();
        _planeStartPoint = new Vector2(_leftDownPointBorder.position.x + _offset,
            _leftDownPointBorder.position.z + _offset);
        _radius = Vector2.Distance(_centerPositionXZ, _planeStartPoint);

        _participant.RopeTouchDuringMoving += KillMoving;
    }

    private void Update()
    {
        if (IsOutField()) // && IsMoving == false)
        {
            // if (IsMoving)
            // {
            //     IsMoving = false;
            //     KillMoving();
            //     return;
            // }
            // Debug.Log("IsOutField = TRUE");
            if (IsPushing == false)
            {
                Debug.Log("IsPushing = FALSE");
                if (_isBoosting == false)
                {
                    Debug.Log("IsBoosting = FALSE");
                    _isTouchBreak = false;
                    _discardingDirection = GetDiscardingDirection(_moveDirection).normalized;
                    _startPosition = transform.position;
                    _newPosition = transform.position + _discardingDirection * _pushDistanceKoef;
                    _pushingDistance = Vector3.Distance(_startPosition, _newPosition);
                    Debug.Log("SAS _pushingDistance " + _pushingDistance);
                    Debug.Log("SAS _pushingDistance " + _pushTime);

                    _turnOverSequence = DOTween.Sequence();
                    _angleRotation = GetTurnOverAngle(_discardingDirection);
                    // Debug.Log("_angleRotation " + _angleRotation);

                    _turnOverSequence.Append(transform
                        .DOLocalRotate(transform.rotation.eulerAngles + new Vector3(0, _angleRotation, 0),
                            _turnOverTime)
                        .SetEase(Ease.Linear));
                    
                    _turnOverSequence.Insert(0, transform
                        .DOMove(transform.position - _discardingDirection * 0.15f, _turnOverTime)
                        .SetEase(Ease.Linear));
                    
                    RotateBeforePushing(new Vector2(-_angleRotateBeforePushing, 0), _turnOverSequence);

                    _turnOverSequence.Append(transform.DOMove(_newPosition, _pushTime).SetEase(Ease.Flash)); //to do uncomment <<==
                    
                    StartCoroutine(StartRunAnimation(_turnOverTime + _preparingPushTime, _pushTime));
                    _boostCoroutine = StartCoroutine(StartBoost(_turnOverTime + _preparingPushTime + _pushTime));
                    StartCoroutine(Reset(_turnOverTime + _preparingPushTime + _pushTime + 0.025f));
                    _isRuling = false;
                    IsPushing = true;
                }
                else
                {
                    _discardingDirection = GetDiscardingDirection(_moveDirection).normalized;
                    Vector3 newPosition = transform.position + _discardingDirection * _pushDistanceKoef / 5;
                    transform.DOMove(newPosition, _pushTime).SetEase(Ease.Flash);
                }
            }
            else
            {
                Debug.Log("PUSHING == true ");
            }
        }
        // else if (IsOutField() && IsPushing)
        // {
        //     Debug.Log("_turnOverSequence KILLED");
        //     _turnOverSequence.Kill();
        //     StartCoroutine(Reset(0));
        // }
        // else if (IsMoving)
        // {
        //     transform.position += _discardingDirection * _pushDistanceKoef;
        // }
    }

    private IEnumerator Push(float runAnimationDelay, float runningTime)
    {
        yield return new WaitForSeconds(runAnimationDelay);
        IsMoving = true;
        IsPushing = true;
        _fxContainer.PlayParticipantEffects();
        _animator.SetFloat("Speed", 2f);
        yield return new WaitForSeconds(runningTime);
        _animator.SetFloat("Speed", _speed);
        IsMoving = false;
        IsPushing = true;
    }

    private void FixedUpdate()
    {
        if (IsMoving)
        {
            // var distanceThisFrame = _pushingDistance/ (_pushTime * 50);
            // // var distanceThisFrame = Time.deltaTime * _pushingDistance * _speed/ (_pushTime);
            // var dir = distanceThisFrame * _discardingDirection;
            // // // var distanceThisFrame = Time.deltaTime * _speed * 10 * _discardingDirection;
            //  Debug.Log("DOC distanceThisFrame : " + dir.magnitude);
            // // Debug.Log("DOC _speed : " + _speed );
            // // Debug.Log("DOC _resultDistance : " + _speed * _pushTime );
            // // transform.Translate(dir, Space.World);
            // transform.position += dir;
            // _movingCounter += Time.deltaTime;
            //
            // if (_movingCounter >= _pushTime)
            // {
            //     Debug.Log("_movingCounter : " + _movingCounter);
            //     IsMoving = false;
            //     _movingCounter = 0;
            // }
            
            
            
            //
            // if (IsOutsideMovingArea(new Vector2(transform.position.x, transform.position.z)) == false)
            // {
            //     IsMoving = false;
            //     _movingCounter = 0;
            // }
        }
    }

    private void KillMoving()
    {
        Debug.Log("_turnOverSequence KILLED");
        _turnOverSequence.Kill();
        _moveDirection = _discardingDirection;
    }

    private bool IsOutsideMovingArea(Vector2 positionXZ)
    {
        return Vector2.Distance(_centerPositionXZ, positionXZ) > (_radius - _offset);
    }

    private float GetTurnOverAngle(Vector3 discardingDirection)
    {
        if (_isLeftBorder && _moveDirection.z > 0)
            return 2 * Vector3.Angle(Vector3.forward, discardingDirection);

        if (_isLeftBorder && _moveDirection.z < 0)
            return -2 * Vector3.Angle(Vector3.back, discardingDirection);

        if (_isRightBorder && _moveDirection.z > 0)
            return -2 * Vector3.Angle(Vector3.forward, discardingDirection);

        if (_isRightBorder && _moveDirection.z < 0)
            return 2 * Vector3.Angle(Vector3.back, discardingDirection);

        if (_isUpBorder && _moveDirection.x > 0)
            return 2 * Vector3.Angle(Vector3.right, discardingDirection);

        if (_isUpBorder && _moveDirection.x < 0)
            return -2 * Vector3.Angle(Vector3.left, discardingDirection);

        if (_isDownBorder && _moveDirection.x > 0)
            return -2 * Vector3.Angle(Vector3.right, discardingDirection);

        if (_isDownBorder && _moveDirection.x < 0)
            return 2 * Vector3.Angle(Vector3.left, discardingDirection);

        return 0;
    }

    private void TryForceMoving(Vector3 newPosition, float duration)
    {
        if (Vector2.Distance(_centerPositionXZ, new Vector2(newPosition.x, newPosition.z)) <= _radius - _offset)
            IsMoving = true;
        else
            IsMoving = false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(new Vector3(_centerPositionXZ.x, transform.position.y, _centerPositionXZ.y),
            _radius - _offset);
    }

    private Vector2 GetRingCenter()
    {
        Vector2 startPosition = new Vector2(_leftDownPointBorder.position.x, _leftDownPointBorder.position.z);
        Vector2 endPosition = new Vector2(_rightUpPointBorder.position.x, _rightUpPointBorder.position.z);

        return new Vector2((endPosition.x - startPosition.x) / 2 + startPosition.x,
            (endPosition.y - startPosition.y) / 2 + startPosition.y);
    }

    private IEnumerator AddForce(float delay, float movingTime)
    {
        yield return new WaitForSeconds(delay);
        // Debug.Log("DAD START");
        _fxContainer.PlayParticipantEffects();
        _rigidbody.isKinematic = false;
        _rigidbody.AddForce(_discardingDirection * 5000, ForceMode.Acceleration);
        yield return new WaitForSeconds(movingTime);
        _rigidbody.isKinematic = true;
    }

    private void RotateBeforePushing(Vector2 angle, Sequence seq)
    {
        seq.Append(transform
            .DOLocalRotate(transform.rotation.eulerAngles + new Vector3(angle.x, _angleRotation, angle.y),
                _preparingPushTime / 2)
            .SetEase(Ease.Linear));

        seq.Append(transform
            .DOLocalRotate(transform.rotation.eulerAngles + new Vector3(0, _angleRotation, 0), _preparingPushTime / 2)
            .SetEase(Ease.Linear));
    }

    private bool IsOutField()
    {
        var position = transform.position;

        if (position.x < _leftDownPointBorder.position.x - _offsetToPush)
        {
            _isLeftBorder = true;
            return true;
        }

        if (position.x > _rightUpPointBorder.position.x + _offsetToPush)
        {
            _isRightBorder = true;
            return true;
        }

        if (position.z < _leftDownPointBorder.position.z - _offsetToPush)
        {
            _isDownBorder = true;
            return true;
        }

        if (position.z > _rightUpPointBorder.position.z + _offsetToPush)
        {
            _isUpBorder = true;
            return true;
        }

        return false;
    }

    private Vector3 GetDiscardingDirection(Vector3 direction)
    {
        Vector3 position = transform.position;
        float positionX = position.x;
        float positionZ = position.z;

        if (positionX < _leftDownPointBorder.position.x + 0.5f || positionX > _rightUpPointBorder.position.x - 0.5f)
            return new Vector3(-direction.x, direction.y, direction.z);
        if (positionZ < _leftDownPointBorder.position.z + 0.5f || positionZ > _rightUpPointBorder.position.z - 0.5f)
            return new Vector3(direction.x, direction.y, -direction.z);

        return Vector3.zero;
    }

    private IEnumerator Reset(float delay)
    {
        yield return new WaitForSeconds(delay);
        _rigidbody.isKinematic = true;
        _isRuling = true;
        _startPosition = Vector3.zero;
        IsPushing = false;
        if (_isTouchBreak)
        {
            _animator.SetFloat("Speed", 0);
            _speed = 0;
        }

        _isDownBorder = false;
        _isUpBorder = false;
        _isLeftBorder = false;
        _isRightBorder = false;
        IsMoving = false;
    }

    private IEnumerator StartBoost(float delay)
    {
        _rigidbody.isKinematic = true;

        if (_isBoosting)
            _speed /= _boost;
            // yield break;

        if (_boostCoroutine != null)
        {
            _speed = _startSpeed;
            StopCoroutine(_boostCoroutine);
        }

        yield return new WaitForSeconds(delay);
        _fxContainer.PlayParticipantEffects();
        _speed *= _boost;
        _isBoosting = true;
        _rigidbody.isKinematic = true;
        // Debug.Log("SAS _speed 1 : " + _speed);
        _animator.SetFloat("Speed", _speed);
        yield return new WaitForSeconds(_boostTime);
        _speed = _startSpeed;
        _isBoosting = false;
        _rigidbody.isKinematic = true;
        _fxContainer.StopParticipantEffects();
        // Debug.Log("DAD SAS _speed 2 : " + _startSpeed);
    }

    private IEnumerator StartRunAnimation(float delayTime, float runnigTime)
    {
        yield return new WaitForSeconds(delayTime);
        // IsMoving = true;
        IsPushing = true;
        // Debug.Log("DAD StartRunAnimation !");
        _fxContainer.PlayParticipantEffects();
        _animator.SetFloat("Speed", 2f);
        yield return new WaitForSeconds(0.05f);
        IsMoving = true;
        yield return new WaitForSeconds(runnigTime);
        // _fxContainer.StopParticipantEffects();
        _animator.SetFloat("Speed", _speed);
        // IsMoving = false;
        // Debug.Log("DAD STOP !");
        yield break;
    }

    public void TryMove(Vector2 direction)
    {
        if (_isRuling == false)
            return;

        Vector2 positionXZ = new Vector2(transform.position.x, transform.position.z);

        if (IsOutsideMovingArea(positionXZ))
        {
            Debug.Log("IsOutsideMovingArea : positionXZ : " + positionXZ);
            Debug.Log("ANGLE: : " + Vector2.Angle(direction, _centerPositionXZ - positionXZ));
            // return;
        }

        if (IsOutsideMovingArea(positionXZ) && (Vector2.Angle(direction, _centerPositionXZ - positionXZ) > 90))
            return;

        Debug.Log("MOVING ");
        _rigidbody.isKinematic = true;

        Rotate(direction);
        Move(direction);
    }

    private void TryRotate(Vector2 direction)
    {
        if (_rotateCounter >= _rotateStep)
        {
            Rotate(direction);
            _rotateCounter = 0;
        }

        _rotateCounter += Time.deltaTime;
    }

    private void Rotate(Vector2 direction)
    {
        _lookRotation = Quaternion.LookRotation(GetWorldDirection(direction));
        transform.rotation = Quaternion.Lerp(transform.rotation, _lookRotation, 0.5f);
    }

    private Vector3 GetWorldDirection(Vector2 direction)
    {
        return new Vector3(direction.x, _positionY, direction.y);
    }

    private void Move(Vector2 direction)
    {
        _moveDirection = new Vector3(direction.x, 0, direction.y);

        if (_speed < _startSpeed)
        {
            _speed = _isBoosting ? _boost * _startSpeed : _startSpeed;
        }

        _animator.SetFloat("Speed", _moveDirection.normalized.magnitude * _speed);
        transform.position += Time.deltaTime * _speed * 2 * _moveDirection.normalized;
    }

    public void StopMoving()
    {
        if (IsPushing == false)
        {
            // Debug.Log("SAS speed and anim speed  = 0");
            _rigidbody.velocity = Vector3.zero;
            _speed = 0;
            _animator.SetFloat("Speed", 0);
        }
        else
        {
            _isTouchBreak = true;
            // Debug.Log("SAS Мы отжали кнопку мыши во время толчка");
        }
    }

    private void AddForceFly(Participant participant)
    {
        // _rigidbody.constraints = RigidbodyConstraints.None;
        // // _rigidbody.constraints == RigidbodyConstraints.FreezePosition 
        // AddForce(new Vector3(1, 25, 1));
        // _rigidbody.isKinematic = false;

        //===================================================

        // Camera.main.gameObject.GetComponent<CameraMover>().SetTrack(participant.gameObject.transform);

        var startPosition = participant.gameObject.transform.position;
        // Sequence sequence = DOTween.Sequence();
        // sequence.Append(participant.transform.DOMove(new Vector3(3, 5, 3), 1).SetEase(Ease.Linear));
        // sequence.Append(participant.transform.DOMove(new Vector3(startPosition.x, startPosition.y, startPosition.z), 1).SetEase(Ease.Linear));
        // participant.transform.DOMove(new Vector3(1, 3, 1), 1);
        // participant.transform.DOMove(new Vector3(3, -9, 3), 3);
    }
}