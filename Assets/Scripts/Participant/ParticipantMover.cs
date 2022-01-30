using System.Collections;
using DG.Tweening;
using UnityEngine;

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
    public bool IsBoosting { get; private set; }
    public bool IsFlying { get; private set; }
    public Vector3 NewPosition { get; private set; }
    public Vector3 MovingDirection { get; private set; }
    public float Speed => _speed;
    public float BoostTime => _boostTime;
    public float RepulsionTime => _turnOverTime + _preparingPushTime + _pushTime; 

    private bool _isRuling;
    private float _positionY;
    private float _startSpeed;
    private Rigidbody _rigidbody;
    private Participant _participant;
    private Coroutine _boostCoroutine;
    private Animator _animator;
    private Quaternion _lookRotation;
    private Vector3 _startPosition;
    private float _turnOverTime;
    private float _preparingPushTime;
    private float _offsetToPush;
    private float _pushDistanceKoef;
    private float _pushTime;
    private bool _isTouchBreak;
    private Vector3 _discardingDirection;
    private float _angleRotateBeforePushing;
    private float _angleRotation;
    private bool _isLeftBorder;
    private bool _isRightBorder;
    private bool _isUpBorder;
    private bool _isDownBorder;
    private float _radius;
    private float _offsetMovingArea;
    private Vector2 _planeStartPoint;
    private Vector2 _centerPositionXZ;
    private Sequence _turnOverSequence;
    private Camera _mainCamera;
    private bool _isNotBot;
    private BattleController _battleController;
    private float _flyingTime;

    private void Start()
    {
        IsFlying = false;
        IsMoving = false;
        _isRuling = true;
        _offsetToPush = -0.1f;
        _turnOverTime = 0.2f;
        _preparingPushTime = 0.3f;
        _angleRotateBeforePushing = 15;
        _pushTime = 0.2f;
        _angleRotation = 180;
        _pushDistanceKoef = 2f;
        _positionY = transform.position.y;
        _startSpeed = _speed;
        _rigidbody = GetComponent<Rigidbody>();
        _participant = GetComponent<Participant>();
        _battleController = FindObjectOfType<BattleController>();
        _animator = GetComponentInChildren<Animator>();
        _participant.SetBoostEffectsVisibility(false);
        if (GetComponent<PlayerInput>() != null)
            _animator.SetFloat(AnimatorParticipantController.Params.Speed, 0f);
        _flyingTime = _battleController.ParticipantFlyingTime;
        IsPushing = false;
        _isTouchBreak = false;
        IsBoosting = false;
        _offsetMovingArea = 0.35f;
        _centerPositionXZ = GetRingCenter();
        _planeStartPoint = new Vector2(_leftDownPointBorder.position.x + _offsetMovingArea,
            _leftDownPointBorder.position.z + _offsetMovingArea);
        _radius = Vector2.Distance(_centerPositionXZ, _planeStartPoint);
        _mainCamera = Camera.main;
        _isNotBot = GetComponent<Bot>() is null;
    }

    private void Update()
    {
        if (IsFlying)
            return;

        if (IsOutsideMovingArea(new Vector2(transform.position.x, transform.position.z)))
        {
            if (IsOutField(transform.position, out TouchBorder startTouchBorder))
            {
                float offset = 0.35f;
                if (startTouchBorder == TouchBorder.LEFT)
                    transform.position += new Vector3(offset, 0, 0);
                else if (startTouchBorder == TouchBorder.RIGHT)
                    transform.position += new Vector3(-offset, 0, 0);
                else if (startTouchBorder == TouchBorder.UP)
                    transform.position += new Vector3(0, 0, -offset);
                else if (startTouchBorder == TouchBorder.DOWN)
                    transform.position += new Vector3(0, 0, offset);
            }

            _turnOverSequence.Kill();
        }


        if (_isNotBot == false)
            return;

        if (IsOutField(transform.position, out TouchBorder touchBorder))
        {
            if (IsPushing == false)
            {
                DoRepulsion(MovingDirection, touchBorder);
            }
        }
    }

    public void DoRepulsion(Vector3 moveDirection, TouchBorder touchBorder, bool isBot = false)
    {
        if (IsBoosting == false)
        {
            DoRopeRepulsion(moveDirection, touchBorder);
        }
        else
        {
            StopBoost();
            DoRopeRepulsion(moveDirection, touchBorder);
        }
    }

    public void DoRopeRepulsion(Vector3 moveDirection, TouchBorder touchBorder, bool isBot = false)
    {
        _discardingDirection = GetDiscardingDirection(moveDirection).normalized;
        _startPosition = transform.position;
        _pushDistanceKoef = isBot ? _pushDistanceKoef * 0.75f : _pushDistanceKoef;
        NewPosition = _startPosition + _discardingDirection * _pushDistanceKoef;
        // _pushingDistance = Vector3.Distance(_startPosition, NewPosition);
        
        _turnOverSequence = DOTween.Sequence();
        _angleRotation = GetTurnOverAngle(moveDirection, _discardingDirection, touchBorder);

        _turnOverSequence.Append(transform
            .DOLocalRotate(transform.rotation.eulerAngles + new Vector3(0, _angleRotation, 0),
                _turnOverTime)
            .SetEase(Ease.Linear));
        _turnOverSequence.Insert(0, transform
            .DOMove(transform.position - _discardingDirection * 0.15f, _turnOverTime)
            .SetEase(Ease.Linear));
        RotateBeforePushing(new Vector2(-_angleRotateBeforePushing, 0), _turnOverSequence);
        _turnOverSequence.Append(transform.DOMove(NewPosition, isBot ? _pushTime * 0.75f : _pushTime)
            .SetEase(Ease.Flash)); 

        StartCoroutine(StartRunAnimation(_turnOverTime + _preparingPushTime, _pushTime));
        _boostCoroutine = StartCoroutine(StartBoost(_turnOverTime + _preparingPushTime + _pushTime));
        StartCoroutine(Reset(_turnOverTime + _preparingPushTime + _pushTime + 0.025f));
        _isRuling = false;
        IsPushing = true;
    }

    public bool IsOutsideMovingArea(Vector2 positionXZ)
    {
        return Vector2.Distance(_centerPositionXZ, positionXZ) > (_radius - _offsetMovingArea);
    }

    private float GetTurnOverAngle(Vector3 moveDirection, Vector3 discardingDirection, TouchBorder touchBorder)
    {
        if (touchBorder == TouchBorder.LEFT && moveDirection.z > 0)
            return 2 * Vector3.Angle(Vector3.forward, discardingDirection);

        if (touchBorder == TouchBorder.LEFT && moveDirection.z < 0)
            return -2 * Vector3.Angle(Vector3.back, discardingDirection);

        if (touchBorder == TouchBorder.RIGHT && moveDirection.z > 0)
            return -2 * Vector3.Angle(Vector3.forward, discardingDirection);

        if (touchBorder == TouchBorder.RIGHT && moveDirection.z < 0)
            return 2 * Vector3.Angle(Vector3.back, discardingDirection);

        if (touchBorder == TouchBorder.UP && moveDirection.x > 0)
            return 2 * Vector3.Angle(Vector3.right, discardingDirection);

        if (touchBorder == TouchBorder.UP && moveDirection.x < 0)
            return -2 * Vector3.Angle(Vector3.left, discardingDirection);

        if (touchBorder == TouchBorder.DOWN && moveDirection.x > 0)
            return -2 * Vector3.Angle(Vector3.right, discardingDirection);

        if (touchBorder == TouchBorder.DOWN && moveDirection.x < 0)
            return 2 * Vector3.Angle(Vector3.left, discardingDirection);

        return 0;
    }

    private Vector2 GetRingCenter()
    {
        Vector2 startPosition = new Vector2(_leftDownPointBorder.position.x, _leftDownPointBorder.position.z);
        Vector2 endPosition = new Vector2(_rightUpPointBorder.position.x, _rightUpPointBorder.position.z);

        return new Vector2((endPosition.x - startPosition.x) / 2 + startPosition.x,
            (endPosition.y - startPosition.y) / 2 + startPosition.y);
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

    public bool IsOutField(Vector3 position, out TouchBorder touchBorder)
    {
        if (position.x < _leftDownPointBorder.position.x - _offsetToPush)
        {
            _isLeftBorder = true;
            touchBorder = TouchBorder.LEFT;
            return true;
        }

        if (position.x > _rightUpPointBorder.position.x + _offsetToPush)
        {
            _isRightBorder = true;
            touchBorder = TouchBorder.RIGHT;
            return true;
        }

        if (position.z < _leftDownPointBorder.position.z - _offsetToPush)
        {
            _isDownBorder = true;
            touchBorder = TouchBorder.DOWN;
            return true;
        }

        if (position.z > _rightUpPointBorder.position.z + _offsetToPush)
        {
            touchBorder = TouchBorder.UP;
            _isUpBorder = true;
            return true;
        }

        touchBorder = TouchBorder.NULL;
        return false;
    }

    private Vector3 GetDiscardingDirection(Vector3 direction)
    {
        Vector3 position = transform.position;
        float positionX = position.x;
        float positionZ = position.z;

        if (positionX < _leftDownPointBorder.position.x + 0.65f || positionX > _rightUpPointBorder.position.x - 0.65f)
        {
            return new Vector3(-direction.x, direction.y, direction.z);
        }

        if (positionZ < _leftDownPointBorder.position.z + 0.65f || positionZ > _rightUpPointBorder.position.z - 0.65f)
        {
            return new Vector3(direction.x, direction.y, -direction.z);
        }

        return Vector3.zero;
    }

    public IEnumerator Reset(float delay)
    {
        yield return new WaitForSeconds(delay);
        _rigidbody.isKinematic = true;
        _isRuling = true;
        _startPosition = Vector3.zero;
        IsPushing = false;
        if (_isTouchBreak)
        {
            _animator.SetFloat(AnimatorParticipantController.Params.Speed, 0);
            _speed = 0;
        }

        _isDownBorder = false;
        _isUpBorder = false;
        _isLeftBorder = false;
        _isRightBorder = false;
        IsMoving = false;
    }

    public void StopBoost()
    {
        if (_boostCoroutine != null)
            StopCoroutine(_boostCoroutine);

        _speed = _startSpeed;
        _animator.SetFloat(AnimatorParticipantController.Params.Speed, _speed);
        IsBoosting = false;
        _isRuling = true;
        IsPushing = false;
        _participant.SetBoostEffectsVisibility(false);
    }

    private IEnumerator StartBoost(float delay)
    {
        _rigidbody.isKinematic = true;

        if (IsBoosting)
            _speed /= _boost;

        if (_boostCoroutine != null)
        {
            _speed = _startSpeed;
            StopCoroutine(_boostCoroutine);
        }

        yield return new WaitForSeconds(delay);
        _participant.SetBoostEffectsVisibility(true);
        _speed *= _boost;
        IsBoosting = true;
        _rigidbody.isKinematic = true;
        _animator.SetFloat(AnimatorParticipantController.Params.Speed, _speed);
        yield return new WaitForSeconds(_boostTime);
        _speed = _startSpeed;
        IsBoosting = false;
        _rigidbody.isKinematic = true;
        _participant.SetBoostEffectsVisibility(false);
        _animator.SetFloat(AnimatorParticipantController.Params.Speed, _speed);
    }

    private IEnumerator StartRunAnimation(float delayTime, float runnigTime)
    {
        yield return new WaitForSeconds(delayTime);
        IsPushing = true;
        _participant.SetBoostEffectsVisibility(true);
        _animator.SetFloat(AnimatorParticipantController.Params.Speed, 2f);
        yield return new WaitForSeconds(0.05f);
        IsMoving = true;
        yield return new WaitForSeconds(runnigTime);
        yield break;
    }

    public void TryMove(Vector2 direction)
    {
        if (_isRuling == false)
            return;

        Vector2 positionXZ = new Vector2(transform.position.x, transform.position.z);

        if (IsOutsideMovingArea(positionXZ) && (Vector2.Angle(direction, _centerPositionXZ - positionXZ) > 90))
            return;

        Rotate(direction);
        Move(direction);
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
        MovingDirection = new Vector3(direction.x, 0, direction.y);

        if (_speed < _startSpeed)
        {
            _speed = IsBoosting ? _boost * _startSpeed : _startSpeed;
        }

        _animator.SetFloat(AnimatorParticipantController.Params.Speed, MovingDirection.normalized.magnitude * _speed);
        transform.position += Time.deltaTime * _speed * 2 * MovingDirection.normalized;
    }

    public void StopMoving()
    {
        if (IsPushing == false)
        {
            _rigidbody.velocity = Vector3.zero;
            _speed = 0;
            _animator.SetFloat(AnimatorParticipantController.Params.Speed, 0);
        }
        else
        {
            _isTouchBreak = true;
        }
    }

    public void Fly(Vector3 direction, bool isCameraMoving)
    {
        Vector3 directionWithoutY = new Vector3(direction.x, 0, direction.z);
        if (isCameraMoving)
        {
            CameraMover cameraMover = _mainCamera.gameObject.GetComponent<CameraMover>();
            cameraMover.SetKindMoving(false);
            cameraMover.SetTarget(transform);
        }

        IsFlying = true;
        _animator.SetBool(AnimatorParticipantController.Params.Fly, true);

        var startPosition = transform.position;
        var heihgestPosition = startPosition + directionWithoutY.normalized * 8 + new Vector3(0, 5, 0);
        var endPosition = heihgestPosition + directionWithoutY.normalized * 16 + new Vector3(0, -10, 0);
        
        Sequence sequence = DOTween.Sequence();
        sequence.Append(transform.DOMove(heihgestPosition, _flyingTime / 3).SetEase(Ease.Linear));
        sequence.Append(transform.DOMove(endPosition, 2 * _flyingTime / 3).SetEase(Ease.Linear));
        StartCoroutine(CheckBottleEnded(_flyingTime));
    }

    private IEnumerator CheckBottleEnded(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (_battleController.IsBottleEnded() == false)
            Destroy(gameObject);
    }
}