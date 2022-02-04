using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(ParticipantMover), typeof(BotMover))]
public class BotRuler : MonoBehaviour
{
    public Vector3 MovingDirection { get; private set; }

    private ParticipantMover _participantMover;
    private FoodGeneration _foodGeneration;
    private BorderChecker _borderChecker;
    private BattleController _battleController;
    private FoodUtils _foodUtils;
    private Transform _target;
    private Quaternion _lookRotation;
    private Animator _animator;
    private Vector3 _newPosition;
    private TouchBorder _touchBorder;
    private bool _isRepulsion;
    private Participant[] _participants;
    private bool _isRopeNextTo;
    private BotMover _botMover;

    private void Start()
    {
        Reset();

        _participantMover = GetComponent<ParticipantMover>();
        _botMover = GetComponent<BotMover>();
        _foodGeneration = FindObjectOfType<FoodGeneration>();
        _borderChecker = FindObjectOfType<BorderChecker>();
        _battleController = FindObjectOfType<BattleController>();
        _foodUtils = FindObjectOfType<FoodUtils>();
        _animator = GetComponentInChildren<Animator>();
        _animator.SetFloat(AnimatorParticipantController.Params.Speed, _participantMover.Speed);
        
        GetComponent<Participant>().FoodEatenByBot += OnFoodEaten;

        _target = _foodUtils.GetNearestFood(transform).transform;
        InvokeRepeating(nameof(TryRotate), 0.5f, 0.1f);
    }

    private void Update()
    {
        if (_isRepulsion)
            return;

        if (_participantMover.IsFlying)
            return;

        if (Vector3.Distance(transform.position, _newPosition) < 0.35f)
        {
            _newPosition = new Vector3(Mathf.Infinity, Mathf.Infinity);
            _participantMover.DoRepulsion(MovingDirection * 100, _touchBorder, true);
            _target.position = _participantMover.NewPosition;
            _isRepulsion = true;
            Invoke(nameof(SetParticipantDirection), _participantMover.RepulsionTime);
            Invoke(nameof(SetNewTarget), _participantMover.RepulsionTime + _participantMover.BoostTime);
        }

        if (_target != null)
        {
            bool isParticipant = _target.TryGetComponent(out ParticipantMover participantMover);
            if (isParticipant && participantMover.IsFlying == false || isParticipant == false)
            {
                MovingDirection = (_target.position - transform.position).normalized;
                transform.Translate(Time.deltaTime * _participantMover.Speed * MovingDirection, Space.World);
            }
            else
            {
                _target = _foodUtils.GetNearestFood(transform);
            }
        }
        else
        {
            _target = _foodUtils.GetNearestFood(transform);
        }
    }

    private void Reset()
    {
        _isRepulsion = false;
        _isRopeNextTo = false;
        _newPosition = new Vector3(Mathf.Infinity, Mathf.Infinity);
        MovingDirection = Vector3.zero;
    }

    private void OnFoodEaten(Food food)
    {
        if (IsRopeNextTo(out TouchBorder touchBorder) == false && _isRopeNextTo == false &&
            _borderChecker.IsNextToAngle(transform.position, 2) == false)
        {
            Invoke(nameof(SetNewTarget), Time.deltaTime);
        }
        else if (_isRopeNextTo == false && _borderChecker.IsNextToAngle(transform.position, 2) == false)
        {
            _isRopeNextTo = true;
            Transform targetTransform = new GameObject().transform;
            targetTransform.position = _newPosition;
            _target = targetTransform;
            _touchBorder = touchBorder;
            Destroy(targetTransform.gameObject, 5);
        }
        else if (_isRopeNextTo == false && _borderChecker.IsNextToAngle(transform.position, 2))
        {
            Invoke(nameof(SetNewTarget), Time.deltaTime);
        }
    }

    private bool IsRopeNextTo(out TouchBorder touchBorder)
    {
        if (_borderChecker.IsOutsideMovingArea(new Vector2(transform.position.x, transform.position.z)))
        {
            touchBorder = TouchBorder.NULL;
            return false;
        }

        Vector3 newPosition = 1.3f * MovingDirection.normalized + transform.position;
        if (_borderChecker.IsOutField(newPosition, out TouchBorder touchBorder2))
        {
            _newPosition = newPosition;
            touchBorder = touchBorder2;
            return true;
        }

        touchBorder = TouchBorder.NULL;
        return false;
    }

    private void SetNewTarget()
    {
        _isRopeNextTo = false;
        _target = _foodUtils.GetNearestFood(transform);
    }

    private void SetParticipantDirection()
    {
        _target = _battleController.GetNearestParticipant(gameObject.GetComponent<Bot>()).gameObject.transform;
        _isRepulsion = false;
    }

    private void TryRotate()
    {
        if (_target == null)
            return;

        if (_isRepulsion)
            return;

        if (_participantMover.IsFlying)
            return;

        _lookRotation = Quaternion.LookRotation(_target.position - transform.position);
        transform.rotation = Quaternion.Lerp(transform.rotation, _lookRotation, 0.85f);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        if (_target != null) Gizmos.DrawWireSphere(_target.transform.position, 0.5f);

        Gizmos.color = Color.green;
        if (_newPosition != null) Gizmos.DrawWireSphere(_newPosition, 0.35f);
    }
}