using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(ParticipantMover))]
public class BotRuler : MonoBehaviour
{
    public Vector3 MovingDirection { get; private set; }

    private ParticipantMover _participantMover;
    private FoodGeneration _foodGeneration;
    private BorderChecker _borderChecker;
    private Food[] _foods;
    private Food _nearestFood;
    private Transform _target;
    private Quaternion _lookRotation;
    private Animator _animator;
    private Vector3 _newPosition;
    private TouchBorder _touchBorder;
    private bool _isRepulsion;
    private Participant[] _participants;
    private bool _isRopeNextTo;

    private void Start()
    {
        _isRepulsion = false;
        _isRopeNextTo = false;
        _newPosition = new Vector3(Mathf.Infinity, Mathf.Infinity);
        MovingDirection = Vector3.zero;
        _participantMover = GetComponent<ParticipantMover>();
        _foodGeneration = FindObjectOfType<FoodGeneration>();
        _borderChecker = FindObjectOfType<BorderChecker>();
        _animator = GetComponentInChildren<Animator>();
        _animator.SetFloat(AnimatorParticipantController.Params.Speed, _participantMover.Speed);
        GetComponent<Participant>().FoodEatenByBot += OnFoodEaten;
        _target = GetNearestFood().transform;

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
                _target = GetNearestFood();
            }
        }
        else
        {
            _target = GetNearestFood();
        }
    }

    private void OnFoodEaten(Food food)
    {
        List<Food> temp = _foods.ToList();
        temp.Clear();
        _foods = temp.ToArray();

        if (IsRopeNextTo(out TouchBorder touchBorder) == false && _isRopeNextTo == false && _borderChecker.IsNextToAngle(transform.position,2) == false)
        {
            Invoke(nameof(SetNewTarget), Time.deltaTime);
        }
        else if (_isRopeNextTo == false && _borderChecker.IsNextToAngle(transform.position,2) == false)
        {
            _isRopeNextTo = true;
            Transform targetTransform = new GameObject().transform;
            targetTransform.position = _newPosition;
            _target = targetTransform;
            _touchBorder = touchBorder;
            Destroy(targetTransform.gameObject, 5);
        }
        else if (_isRopeNextTo == false && _borderChecker.IsNextToAngle(transform.position,2))
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
        _target = GetNearestFood();
    }

    private void SetParticipantDirection()
    {
        _participants = FindObjectsOfType<Participant>();
        var listTargetParticipants = _participants.ToList();
        foreach (var participant in _participants)
        {
            if (participant.gameObject == gameObject)
            {
                listTargetParticipants.Remove(participant);
            }
        }

        Participant targetParticipant = listTargetParticipants[0];
        var distanceTarget = Vector3.Distance(transform.position, targetParticipant.gameObject.transform.position);
        foreach (var participant in listTargetParticipants)
        {
            var distance = Vector3.Distance(transform.position, participant.gameObject.transform.position);
            if (distance < distanceTarget)
            {
                targetParticipant = participant;
                distanceTarget = distance;
            }
        }

        _target = targetParticipant.gameObject.transform;
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

    private Transform GetNearestFood()
    {
        _foods = _foodGeneration.gameObject.GetComponentsInChildren<Food>();
        float shortestDistance = Mathf.Infinity;
        _nearestFood = null;

        foreach (Food food in _foods)
        {
            float distanceToFood = Vector3.Distance(transform.position, food.gameObject.transform.position);
            if (distanceToFood < shortestDistance)
            {
                shortestDistance = distanceToFood;
                _nearestFood = food;
            }
        }

        return _nearestFood != null ? _nearestFood.transform : null;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        if (_target != null) Gizmos.DrawWireSphere(_target.transform.position, 0.5f);

        Gizmos.color = Color.green;
        if (_newPosition != null) Gizmos.DrawWireSphere(_newPosition, 0.35f);
    }
}