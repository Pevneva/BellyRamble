using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(ParticipantMover))]
public class BotRuler : MonoBehaviour
{
    [SerializeField] private Transform _leftDownAngle;
    [SerializeField] private Transform _leftUpAngle;
    [SerializeField] private Transform _rightUpAngle;
    [SerializeField] private Transform _rightDownAngle;
    public Vector3 MovingDirection { get; private set; }

    private ParticipantMover _participantMover;
    private FoodGeneration _foodGeneration;
    private Food[] _foods;
    private Food _nearestFood;
    private Transform _target;
    private Quaternion _lookRotation;
    private Animator _animator;
    private Vector3 _newPosition;
    private TouchBorder _touchBorder;
    private bool _isRepulsion;
    // private bool _isFlying;
    private Participant[] _participants;
    private bool _isRopeNextTo;

    private void Start()
    {
        _isRepulsion = false;
        // _isFlying = false;
        _isRopeNextTo = false;
        _newPosition = new Vector3(Mathf.Infinity, Mathf.Infinity);
        MovingDirection = Vector3.zero;
        _participantMover = GetComponent<ParticipantMover>();
        _foodGeneration = FindObjectOfType<FoodGeneration>();
        _animator = GetComponentInChildren<Animator>();
        _animator.SetFloat("Speed", _participantMover.Speed);
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

        if (Vector3.Distance(transform.position, _newPosition) < 0.15f)
        {
            Debug.Log("QA DoRopeRepulsion MovingDirection : " + MovingDirection);
            _newPosition = new Vector3(Mathf.Infinity, Mathf.Infinity);
            Debug.Log("AAAA DoRopeRepulsion MovingDirection : " + MovingDirection);
            Debug.Log("SAS DoRopeRepulsion _touchBorder : " + _touchBorder);
            Debug.Log("SASA RepulsionTime : " + _participantMover.RepulsionTime);
            Debug.Log("SASA BoostTime : " + _participantMover.BoostTime);
            _participantMover.DoRepulsion(MovingDirection * 100, _touchBorder, true);
            _target.position = _participantMover.NewPosition;
            _isRepulsion = true;
            Invoke(nameof(SetParticipantDirection), _participantMover.RepulsionTime);
            Invoke(nameof(SetNewTarget), _participantMover.RepulsionTime + _participantMover.BoostTime);
        }

        
        if (_target != null)
        {
            MovingDirection = (_target.position - transform.position).normalized;
            transform.Translate(Time.deltaTime * _participantMover.Speed * MovingDirection, Space.World);
        }
        else
        {
            _target = GetNearestFood();
        }
    }

    private void OnFoodEaten(Food food)
    {
        Debug.Log("AAAA Food Eaten");
        List<Food> temp = _foods.ToList();
        temp.Clear();
        _foods = temp.ToArray();

        if (IsRopeNextTo(out TouchBorder touchBorder) == false && _isRopeNextTo == false && IsNextToAngle(2) == false)
        {
            Invoke(nameof(SetNewTarget), Time.deltaTime);
        }
        else if (_isRopeNextTo == false && IsNextToAngle(2) == false)
        {
            Debug.Log("QA OnFoodEaten 1");
            _isRopeNextTo = true;
            Transform targetTransform = new GameObject().transform;
            targetTransform.position = _newPosition;
            _target = targetTransform;
            _touchBorder = touchBorder;
            Destroy(targetTransform.gameObject, 5);
        }
        else if (_isRopeNextTo == false && IsNextToAngle(2))
        {
            Debug.Log("QA OnFoodEaten 2");
            Invoke(nameof(SetNewTarget), Time.deltaTime);
        }
    }
    
    private bool IsRopeNextTo(out TouchBorder touchBorder)
    {
        if (_participantMover.IsOutsideMovingArea(new Vector2(transform.position.x, transform.position.z)))
        {
            touchBorder = TouchBorder.NULL;
            return false;
        }
        
        Debug.Log("AAAA IsRopeNextTo MovingDirection : " + MovingDirection);
        Vector3 newPosition = 1.3f * MovingDirection.normalized + transform.position;
        if (_participantMover.IsOutField(newPosition, out TouchBorder touchBorder2))
        {
            
            Debug.Log("AAAA Rope is next to me ! Border = " + touchBorder2);

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
        
        Debug.Log("SASA SetNewTarget");
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
        var distanceTarget =  Vector3.Distance(transform.position, targetParticipant.gameObject.transform.position);
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
        Debug.Log("AAA SetParticipantDirection _target : " + _target);
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

    private bool IsNextToAngle(float distance)
    {
        if (Vector3.Distance(transform.position, _leftDownAngle.position) < distance)
        {
            Debug.Log("QA distance : " + Vector3.Distance(transform.position, _leftDownAngle.position));
            return true;
        }
        
        if (Vector3.Distance(transform.position, _leftUpAngle.position) < distance)
        {
            Debug.Log("QA distance : " + Vector3.Distance(transform.position, _leftUpAngle.position));
            return true;
        }
        
        if (Vector3.Distance(transform.position, _rightUpAngle.position) < distance)
        {
            Debug.Log("QA distance : " + Vector3.Distance(transform.position, _rightUpAngle.position));
            return true;
        }
        
        if (Vector3.Distance(transform.position, _rightDownAngle.position) < distance)
        {
            Debug.Log("QA distance : " + Vector3.Distance(transform.position, _rightDownAngle.position));
            return true;
        }
        return false;
    }
}