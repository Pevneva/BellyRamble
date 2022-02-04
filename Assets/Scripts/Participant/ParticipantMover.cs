using System.Collections;
using DG.Tweening;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class ParticipantMover : MonoBehaviour
{
    [SerializeField] private float _speed;
    [SerializeField] private float _boost;
    [SerializeField] private float _boostTime = 0.65f;

    public bool IsMoving { get; private set; }
    public bool IsPushing { get; protected set; }
    public bool IsBoosting { get; private set; }
    public bool IsFlying { get; private set; }
    public Vector3 NewPosition { get; protected set; }
    public Vector3 MovingDirection { get; private set; }
    public float Speed => _speed;
    public float BoostTime => _boostTime;
    protected bool IsRuling;

    private float _positionY;
    private float _startSpeed;
    private Rigidbody _rigidbody;
    private Participant _participant;
    private Animator _animator;
    private Quaternion _lookRotation;
    protected Vector3 StartPosition;

    protected Coroutine BoostCoroutine;
    private bool _isTouchBreak;
    protected Sequence TurnOverSequence;
    private Camera _mainCamera;
    private BattleController _battleController;
    private float _flyingTime;

    protected BorderChecker BorderChecker;

    protected void Start()
    {
        IsFlying = false;
        IsMoving = false;
        IsRuling = true;

        _positionY = transform.position.y;
        _startSpeed = _speed;
        _rigidbody = GetComponent<Rigidbody>();
        _participant = GetComponent<Participant>();
        _battleController = FindObjectOfType<BattleController>();
        BorderChecker = FindObjectOfType<BorderChecker>();
        _animator = GetComponentInChildren<Animator>();
        _participant.SetBoostEffectsVisibility(false);
        if (GetComponent<PlayerInput>() != null)
            _animator.SetFloat(AnimatorParticipantController.Params.Speed, 0f);
        _flyingTime = _battleController.ParticipantFlyingTime;
        IsPushing = false;
        _isTouchBreak = false;
        IsBoosting = false;
        _mainCamera = Camera.main;
    }

    protected void Update()
    {
        if (IsFlying)
            return;

        if (BorderChecker.IsOutsideRing(new Vector2(transform.position.x, transform.position.z)))
        {
            if (BorderChecker.IsOutField(transform.position, out TouchBorder startTouchBorder))
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

            TurnOverSequence.Kill();
        }
    }

    public IEnumerator Reset(float delay)
    {
        yield return new WaitForSeconds(delay);
        _rigidbody.isKinematic = true;
        IsRuling = true;
        StartPosition = Vector3.zero;
        IsPushing = false;
        if (_isTouchBreak)
        {
            _animator.SetFloat(AnimatorParticipantController.Params.Speed, 0);
            _speed = 0;
        }

        BorderChecker.ResetBorders();
        IsMoving = false;
    }

    public void StopBoost()
    {
        if (BoostCoroutine != null)
            StopCoroutine(BoostCoroutine);

        _speed = _startSpeed;
        _animator.SetFloat(AnimatorParticipantController.Params.Speed, _speed);
        IsBoosting = false;
        IsRuling = true;
        IsPushing = false;
        _participant.SetBoostEffectsVisibility(false);
    }

    protected IEnumerator StartBoost(float delay)
    {
        _rigidbody.isKinematic = true;

        if (IsBoosting)
            _speed /= _boost;

        if (BoostCoroutine != null)
        {
            _speed = _startSpeed;
            StopCoroutine(BoostCoroutine);
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

    protected IEnumerator StartRunAnimation(float delayTime, float runnigTime)
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
        if (IsRuling == false)
            return;

        Vector2 positionXZ = new Vector2(transform.position.x, transform.position.z);

        if (BorderChecker.IsOutsideRing(positionXZ) &&
            (Vector2.Angle(direction, BorderChecker.CenterPositionXZ - positionXZ) > 90))
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