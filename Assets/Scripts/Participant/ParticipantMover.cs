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
    private Vector3 _delayPosition;
    private float _turnOverTime;
    private float _preparingPushTime;
    private bool _isPushing;
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
    private Vector2 _planeCenterPosition;

    private void Start()
    {
        _isForcing = false;
        _isRuling = true;
        _isDelayingPushing = false;
        _offsetToPush = -0.1f;
        _delayCounter = 0;
        _turnOverTime = 0.2f;
        _preparingPushTime = 0.3f;
        // _preparingPushTime = 3f;
        _delayTime = 0.55f;
        _angleRotateBeforePushing = 15;
        _angleRotation = 180;
        // _pushDistanceKoef = 0.5f;
        _pushDistanceKoef = 2f;
        _pushTime = 0.2f;
        // _pushTime = 0.075f;
        // _boost = 2f;
        _positionY = transform.position.y;
        _startSpeed = _speed;
        _rigidbody = GetComponent<Rigidbody>();
        _participant = GetComponent<Participant>();
        _animator = GetComponentInChildren<Animator>();
        _fxContainer = FindObjectOfType<FXContainer>();
        _fxContainer.StopParticipantEffects();
        _animator.SetFloat("Speed", 0f);

        _isPushing = false;
        _isTouchBreak = false;
        _isBoosting = false;
        _offset = 0.35f;
        _planeCenterPosition = GetRingCenter();
        _planeStartPoint = new Vector2(_leftDownPointBorder.position.x + _offset, _leftDownPointBorder.position.z + _offset);
        _radius = Vector2.Distance(_planeCenterPosition, _planeStartPoint);


        // Sequence seq = DOTween.Sequence();
        // seq.Append(transform.DOLocalRotate(transform.rotation.eulerAngles + new Vector3(-10, 0, 0), 1));
        // seq.Append(transform.DOLocalRotate(transform.rotation.eulerAngles + new Vector3(0, 0, 0), 2));
    }

    private void Update()
    {
        if (IsOutField())
        {
            if (_isPushing == false)
            {
                if (_isBoosting == false)
                {
                    _isTouchBreak = false;
                    // _discardingDirection = GetDiscardingDirection();
                    _discardingDirection = GetDiscardingDirection2(_moveDirection).normalized;
                    // _delayPosition = GetCurrentPosition();
                    Vector3 newPosition = transform.position + _discardingDirection * _pushDistanceKoef;
                    Debug.Log("SAS newPosition " + newPosition);
                    Debug.Log("SAS !!! _moveDirection " + _moveDirection);
                    Debug.Log("SAS !!! isLeftBorder " + _isLeftBorder);
                    Debug.Log("SAS !!! _discardingDirection " + _discardingDirection);

                    Sequence sequence = DOTween.Sequence();

                    if (_isLeftBorder && _moveDirection.z > 0)
                    {
                        Debug.Log("SAS !!! ===== " + Vector3.Angle(Vector3.forward, _discardingDirection));

                        _angleRotation = 2 * Vector3.Angle(Vector3.forward, _discardingDirection);
                    }
                    
                    if (_isLeftBorder && _moveDirection.z < 0)
                        _angleRotation = - 2 * Vector3.Angle(Vector3.back, _discardingDirection);
                    
                    if (_isRightBorder && _moveDirection.z > 0)
                        _angleRotation = - 2 * Vector3.Angle(Vector3.forward, _discardingDirection);
                    
                    if (_isRightBorder  && _moveDirection.z < 0)
                        _angleRotation = 2 * Vector3.Angle(Vector3.back, _discardingDirection);  
                    
                    if (_isUpBorder  && _moveDirection.x > 0)
                        _angleRotation = 2 * Vector3.Angle(Vector3.right, _discardingDirection); 
                    
                    if (_isUpBorder  && _moveDirection.x < 0)
                        _angleRotation = - 2 * Vector3.Angle(Vector3.left, _discardingDirection);  
                    
                    if (_isDownBorder  && _moveDirection.x > 0)
                        _angleRotation = - 2 * Vector3.Angle(Vector3.right, _discardingDirection); 
                    
                    if (_isDownBorder  && _moveDirection.x < 0)
                        _angleRotation = 2 * Vector3.Angle(Vector3.left, _discardingDirection); 
                    
                    
                    Debug.Log("SAS !!! angleRotation : " + _angleRotation);
                    sequence.Append(transform
                        .DOLocalRotate(transform.rotation.eulerAngles + new Vector3(0, _angleRotation, 0),
                            _turnOverTime)
                        // .DOLocalRotate(transform.rotation.eulerAngles + new Vector3(0, -180, 0), _turnOverTime)
                        .SetEase(Ease.Linear));

                    sequence.Insert(0, transform
                        // .DOMove(_delayPosition - _discardingDirection * 0.35f, _turnOverTime)
                        .DOMove(GetCurrentPosition() - _discardingDirection * 0.5f, _turnOverTime)
                        .SetEase(Ease.Linear));

                        RotateBeforePushing(new Vector2(-_angleRotateBeforePushing, 0), sequence);
                        
                    // else if (_discardingDirection == Vector3.right)
                    //     RotateBeforePushing(new Vector2(-_angleRotateBeforePushing, 0), sequence);
                    // else if (_discardingDirection == Vector3.back)
                    //     RotateBeforePushing(new Vector2(-_angleRotateBeforePushing, 0), sequence);
                    // else if (_discardingDirection == Vector3.forward)
                    //     RotateBeforePushing(new Vector2(-_angleRotateBeforePushing, 0), sequence);

                    // sequence.Append(transform.DOMove(newPosition, _pushTime).SetEase(Ease.Flash)); //to do uncomment
                    StartCoroutine(AddForce(_turnOverTime + _preparingPushTime, 0.095f));
                    StartCoroutine(Reset(_turnOverTime + _preparingPushTime + _pushTime + 0.025f));
                    StartCoroutine(StartRunAnimation(_turnOverTime + _preparingPushTime, _pushTime));
                    _boostCoroutine = StartCoroutine(StartBoost(_turnOverTime + _preparingPushTime + _pushTime));
                    _isRuling = false;
                    _isPushing = true;
                }
                else
                {
                    Vector3 newPosition = transform.position + _discardingDirection * _pushDistanceKoef / 5;
                    transform.DOMove(newPosition, _pushTime).SetEase(Ease.Flash);
                }
            }
        }
    }

    private bool TryBlockedDirection(Vector3 leftDownPoint, Vector3 rightUpPoint)
    {
        // _offset = 0.35f;
        // Debug.Log("DOS x blocked 0 : " + (GetRingCenter()));
        Vector3 centerPosition = new Vector3(GetRingCenter().x, transform.position.y, GetRingCenter().y);
        Vector2 startPoint =
            new Vector2(_leftDownPointBorder.position.x + _offset, _leftDownPointBorder.position.z + _offset);
        
        _radius = Vector2.Distance(GetRingCenter(), startPoint);

        if (Vector3.Distance(centerPosition, transform.position) > _radius - _offset)
        {
            Vector3 position = transform.position;
            Debug.Log("DOS x direction : " + (centerPosition - position).normalized);
            Vector3 newPosition = transform.position + (centerPosition - position).normalized * 0.25f;
            Debug.Log("DOS newPosition : " + newPosition);
            // Vector3 newPosition2 = position + new Vector3(0.025f, 0, 0);
            // Debug.Log("DOS x blocked 1 : " + (position - GetRingCenter()));
            // Debug.Log("DOS x blocked 2 : " + (position - GetRingCenter()).normalized);
            
            transform.position = newPosition;
            // Camera.main.gameObject.GetComponent<CameraMover>().IsBlockedMoving = true;
            // Camera.main.gameObject.GetComponent<CameraMover>().IsBlockedMoving = false;
            return true;
        }

        return false;
    }
    
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(new Vector3(_planeCenterPosition.x, transform.position.y, _planeCenterPosition.y), _radius - _offset);
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
        Debug.Log("DAD START");
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

    private Vector3 GetDiscardingDirection2(Vector3 direction)
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

    private Vector3 GetCurrentPosition()
    {
        return transform.position;
    }

    private IEnumerator Reset(float delay)
    {
        yield return new WaitForSeconds(delay);
        _rigidbody.isKinematic = true;
        _isRuling = true;
        _delayPosition = Vector3.zero;
        _isPushing = false;
        if (_isTouchBreak)
        {
            Debug.Log("SAS set speed animation 0 ");
            _animator.SetFloat("Speed", 0);
            _speed = 0;
        }

        _isDownBorder = false;
        _isUpBorder = false;
        _isLeftBorder = false;
        _isRightBorder = false;
        
        Debug.Log("SAS _isTouchBreak = " + _isTouchBreak);
        Debug.Log("SAS _isRuling = " + _isRuling);
    }

    private IEnumerator StartBoost(float delay)
    {
        _rigidbody.isKinematic = true;
        
        if (_isBoosting)
            yield break;

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
        Debug.Log("SAS _speed 1 : " + _speed);
        _animator.SetFloat("Speed", _speed);
        yield return new WaitForSeconds(_boostTime);
        _speed = _startSpeed;
        _isBoosting = false;
        _rigidbody.isKinematic = true;
        _fxContainer.StopParticipantEffects();
        Debug.Log("DAD SAS _speed 2 : " + _startSpeed);
    }

    private IEnumerator StartRunAnimation(float delayTime, float runnigTime)
    {
        yield return new WaitForSeconds(delayTime);
        Debug.Log("DAD StartRunAnimation !");
        // _fxContainer.PlayParticipantEffects();
        _animator.SetFloat("Speed", 2f);
        yield return new WaitForSeconds(runnigTime);
        // _fxContainer.StopParticipantEffects();
        _animator.SetFloat("Speed", _speed);
        Debug.Log("DAD STOP !");
        yield break;
    }

    public void TryMove(Vector2 direction)
    {
        if (_isRuling == false)
            return;

        Vector2 planePosition = new Vector2(transform.position.x, transform.position.z);
        // Debug.Log("DAD 1 :" + Vector2.Distance(_planeCenterPosition, planePosition));
        // Debug.Log("DAD 2 :" + (_radius - _offset));
        
        if ((Vector2.Distance(_planeCenterPosition, planePosition) > (_radius - _offset)) && (Vector2.Angle(direction, _planeCenterPosition - planePosition) > 90))
            return;

        _rigidbody.isKinematic = true;

        Rotate(direction);
        Move(direction);
    }

    private void Rotate(Vector2 direction)
    {
        _lookRotation = Quaternion.LookRotation(GetWorldDirection(direction));
        transform.rotation = Quaternion.Lerp(transform.rotation, _lookRotation, 0.05f);
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
            Debug.Log("SAS set speed _startSpeed : _isBoosting = " + _isBoosting);
            _speed = _isBoosting ? _boost * _startSpeed : _startSpeed;
            Debug.Log("SAS _speed = " + _speed);
            Debug.Log("SAS animation speed = " + _moveDirection.normalized.magnitude * _speed);
        }

        _animator.SetFloat("Speed", _moveDirection.normalized.magnitude * _speed);
        // _animator.SetFloat("Speed", _speed);
        transform.position += Time.deltaTime * _speed * 2 * _moveDirection.normalized;
    }

    public void StopMoving()
    {
        if (_isPushing == false)
        {
            Debug.Log("SAS speed and anim speed  = 0");
            _rigidbody.velocity = Vector3.zero;
            _speed = 0;
            _animator.SetFloat("Speed", 0);
        }
        else
        {
            _isTouchBreak = true;
            Debug.Log("SAS Мы отжали кнопку мыши во время толчка");
        }
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