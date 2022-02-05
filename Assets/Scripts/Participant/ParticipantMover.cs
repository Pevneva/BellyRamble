using System.Collections;
using DG.Tweening;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class ParticipantMover : MonoBehaviour
{
    [SerializeField] private float _speed;
    [SerializeField] private float _boost;
    [SerializeField] private float _boostTime;

    protected bool IsPushing;
    protected bool IsRuling;
    protected Vector3 StartPosition;
    protected Coroutine BoostCoroutine;
    protected BorderChecker BorderChecker;
    protected Sequence TurnOverSequence;
    protected Rigidbody Rigidbody;
    protected Animator Animator;
    protected Participant Participant;
    protected float Boost => _boost;
    protected Vector3 MovingDirection { get; private set; }
    protected float StartSpeed { get; private set; }
    public bool IsBoosting { get; protected set; }
    public bool IsFlying { get; private set; }
    public Vector3 NewPosition { get; protected set; }
    public float BoostTime => _boostTime;
    public float Speed
    {
        get { return _speed; }
        protected set { _speed = value; }
    }
    
    private bool _isMoving;
    private float _positionY;
    private Quaternion _lookRotation;
    private bool _isTouchBreak;
    private Camera _mainCamera;
    private BattleController _battleController;
    private float _flyingTime;

    protected void Start()
    {
        IsFlying = false;
        _isMoving = false;
        IsRuling = true;

        _positionY = transform.position.y;
        StartSpeed = _speed;
        Rigidbody = GetComponent<Rigidbody>();
        Participant = GetComponent<Participant>();
        _battleController = FindObjectOfType<BattleController>();
        BorderChecker = FindObjectOfType<BorderChecker>();
        Animator = GetComponentInChildren<Animator>();
        Participant.SetBoostEffectsVisibility(false);
        if (GetComponent<PlayerInput>() != null)
            Animator.SetFloat(AnimatorParticipantController.Params.Speed, 0f);
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
            TryMoveToRing();
            TurnOverSequence.Kill();
        }
    }

    private void TryMoveToRing()
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
    }

    public IEnumerator Reset(float delay)
    {
        yield return new WaitForSeconds(delay);
        Rigidbody.isKinematic = true;
        IsRuling = true;
        StartPosition = Vector3.zero;
        IsPushing = false;
        if (_isTouchBreak)
        {
            Animator.SetFloat(AnimatorParticipantController.Params.Speed, 0);
            _speed = 0;
        }

        BorderChecker.ResetBorders();
        _isMoving = false;
    }

    protected IEnumerator StartRunAnimation(float delayTime, float runnigTime)
    {
        yield return new WaitForSeconds(delayTime);
        IsPushing = true;
        Participant.SetBoostEffectsVisibility(true);
        Animator.SetFloat(AnimatorParticipantController.Params.Speed, 2f);
        yield return new WaitForSeconds(0.05f);
        _isMoving = true;
        yield return new WaitForSeconds(runnigTime);
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

        if (_speed < StartSpeed)
        {
            _speed = IsBoosting ? _boost * StartSpeed : StartSpeed;
        }

        Animator.SetFloat(AnimatorParticipantController.Params.Speed, MovingDirection.normalized.magnitude * _speed);
        transform.position += Time.deltaTime * _speed * 2 * MovingDirection.normalized;
    }

    public void StopMoving()
    {
        if (IsPushing == false)
        {
            Rigidbody.velocity = Vector3.zero;
            _speed = 0;
            Animator.SetFloat(AnimatorParticipantController.Params.Speed, 0);
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
        Animator.SetBool(AnimatorParticipantController.Params.Fly, true);

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