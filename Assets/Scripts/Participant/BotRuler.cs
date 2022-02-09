using UnityEngine;

[RequireComponent(typeof(ParticipantPusherOut), typeof(BotMover))]
public class BotRuler : MonoBehaviour
{
    private readonly float _spreadDistance = 0.35f;
    private ParticipantPusherOut _participantPusherOut;
    private BorderChecker _borderChecker;
    private BattleController _battleController;
    private BotMover _botMover;
    private FoodUtils _foodUtils;
    private Transform _target;
    private Vector3 _ropePoint;
    private TouchBorder _touchBorder;
    private bool _isPushingOut;
    private Vector3 _movingDirection;

    private void Start()
    {
        Reset();

        _participantPusherOut = GetComponent<ParticipantPusherOut>();
        _botMover = GetComponent<BotMover>();
        
        // _foodUtils = FindObjectOfType<FoodUtils>(); //AAA
        GetComponentInChildren<Animator>().SetFloat(AnimatorParticipantController.Params.Speed, _participantPusherOut.Speed);
        _target = _foodUtils.GetNearestFood(transform);

        InvokeRepeating(nameof(TryRotate), 0.5f, 0.1f);
        GetComponent<Participant>().FoodEatenByBot += OnFoodEaten;
    }

    private void Update()
    {
        if (_isPushingOut)
            return;

        if (_participantPusherOut.IsFlying)
            return;

        if (_borderChecker.IsOutsideRing(new Vector2(transform.position.x, transform.position.z)))
            SetNewTarget();

        if (Vector3.Distance(transform.position, _ropePoint) < _spreadDistance) 
            PushOut();

        if (_target != null)
        {
            bool isParticipant = _target.TryGetComponent(out ParticipantMover participantMover);
            if (isParticipant && participantMover.IsFlying == false || isParticipant == false)
                _botMover.Move(_target, _participantPusherOut.Speed, out _movingDirection);
            else
                _target = _foodUtils.GetNearestFood(transform);
        }
        else
        {
            _target = _foodUtils.GetNearestFood(transform);
        }
    }

    public void Init(BattleController battleController, BorderChecker borderChecker, FoodUtils foodUtils)
    {
        _battleController = battleController;
        _borderChecker = borderChecker;
        _foodUtils = foodUtils;
    }

    private void Reset()
    {
        _isPushingOut = false;
        _ropePoint = new Vector3(Mathf.Infinity, Mathf.Infinity);
        _movingDirection = Vector3.zero;
    }

    private void PushOut()
    {
        _ropePoint = new Vector3(Mathf.Infinity, Mathf.Infinity);
        _participantPusherOut.DoRepulsion(_movingDirection, _touchBorder, true);
        _target.position = _participantPusherOut.NewPosition;
        _isPushingOut = true;
        Invoke(nameof(SetParticipantDirection), _participantPusherOut.RepulsionTime);
        Invoke(nameof(SetNewTarget), _participantPusherOut.RepulsionTime + MovingParamsController.BoostTime);
    }

    private void OnFoodEaten(Food food)
    {
        if (_participantPusherOut.Speed > _participantPusherOut.StartSpeed)
            return;
        
        if (_borderChecker.IsRopeNearby(transform.position) == false ||
            _borderChecker.IsAngleNearby(transform.position, MovingParamsController.IgnoredDistanceToAngle))
        {
            Invoke(nameof(SetNewTarget), Time.deltaTime);
        }
        else
        {
            SetRopePoint();
            SetRopePointTarget();
        }
    }

    private void SetRopePoint()
    {
        Vector3 ropePoint = MovingParamsController.RopePointDistanceKoef * _movingDirection.normalized + transform.position;
        _touchBorder = _borderChecker.TryGetTouchBorder(ropePoint);

        if (_touchBorder == TouchBorder.NULL)
            _ropePoint = Vector3.zero;
        else
            _ropePoint = ropePoint;
    }

    private void SetRopePointTarget()
    {
        if (_ropePoint != Vector3.zero)
        {
            Transform targetTransform = new GameObject().transform;
            targetTransform.position = _ropePoint;
            _target = targetTransform;
            Destroy(targetTransform.gameObject, 5);
        }
        else
        {
            Invoke(nameof(SetNewTarget), Time.deltaTime);
        }
    }

    private void SetNewTarget()
    {
        _target = _foodUtils.GetNearestFood(transform);
    }

    private void SetParticipantDirection()
    {
        _target = _battleController.GetNearestParticipant(gameObject.GetComponent<Bot>()).gameObject.transform;
        
        _isPushingOut = false;
    }

    private void TryRotate()
    {
        if (_target == null)
            return;
    
        if (_isPushingOut)
            return;
    
        if (_participantPusherOut.IsFlying)
            return;

        _botMover.Rotate(_target);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        if (_target != null) Gizmos.DrawWireSphere(_target.transform.position, 0.5f);
    }
}