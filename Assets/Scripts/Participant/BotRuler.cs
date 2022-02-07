using UnityEngine;

[RequireComponent(typeof(ParticipantPusherOut))]
public class BotRuler : MonoBehaviour
{
    public Vector3 MovingDirection { get; private set; }

    private ParticipantPusherOut _participantPusherOut;
    private BorderChecker _borderChecker;
    private BattleController _battleController;
    private FoodUtils _foodUtils;
    private Transform _target;
    private Quaternion _lookRotation;
    private Animator _animator;
    private Vector3 _ropePoint;
    private TouchBorder _touchBorder;
    private bool _isPushingOut;

    private void Start()
    {
        Reset();

        _participantPusherOut = GetComponent<ParticipantPusherOut>();
        _borderChecker = FindObjectOfType<BorderChecker>();
        _battleController = FindObjectOfType<BattleController>();
        _foodUtils = FindObjectOfType<FoodUtils>();
        _animator = GetComponentInChildren<Animator>();
        _animator.SetFloat(AnimatorParticipantController.Params.Speed, _participantPusherOut.Speed);
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

        if (Vector3.Distance(transform.position, _ropePoint) < 0.35f) //AAA do const
            PushOut();

        if (_target != null)
        {
            bool isParticipant = _target.TryGetComponent(out ParticipantMover participantMover);
            if (isParticipant && participantMover.IsFlying == false || isParticipant == false)
                Move();
            else
                _target = _foodUtils.GetNearestFood(transform);
        }
        else
        {
            _target = _foodUtils.GetNearestFood(transform);
        }
    }

    private void Reset()
    {
        _isPushingOut = false;
        _ropePoint = new Vector3(Mathf.Infinity, Mathf.Infinity);
        MovingDirection = Vector3.zero;
    }

    private void PushOut()
    {
        _ropePoint = new Vector3(Mathf.Infinity, Mathf.Infinity);
        _participantPusherOut.DoRepulsion(MovingDirection, _touchBorder, true);
        _target.position = _participantPusherOut.NewPosition;
        _isPushingOut = true;
        Invoke(nameof(SetParticipantDirection), _participantPusherOut.RepulsionTime);
        Invoke(nameof(SetNewTarget), _participantPusherOut.RepulsionTime + MovingParamsController.BoostTime);
    }

    private void Move()
    {
        MovingDirection = (_target.position - transform.position).normalized;
        transform.Translate(Time.deltaTime * _participantPusherOut.Speed * MovingDirection, Space.World);
    }

    private void OnFoodEaten(Food food)
    {
        if (IsRopeNearby() == false ||
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

    private bool IsRopeNearby()
    {
        var colliders = Physics.OverlapSphere(transform.position, MovingParamsController.ProcessedDistanceToRope);

        foreach (var collider in colliders)
            if (collider.gameObject.TryGetComponent<Rope>(out Rope rope))
                return true;
        
        return false;
    }

    private void SetRopePoint()
    {
        Vector3 ropePoint = MovingParamsController.RopePointDistanceKoef * MovingDirection.normalized + transform.position;
        _touchBorder = TryGetTouchBorder(ropePoint);

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

    private TouchBorder TryGetTouchBorder(Vector3 position)
    {
        if (_borderChecker.IsOutField(position, out TouchBorder touchBorder))
            return touchBorder;

        return TouchBorder.NULL;
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

        _lookRotation = Quaternion.LookRotation(_target.position - transform.position);
        transform.rotation = Quaternion.Lerp(transform.rotation, _lookRotation, 0.85f);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        if (_target != null) Gizmos.DrawWireSphere(_target.transform.position, 0.5f);
    }
}