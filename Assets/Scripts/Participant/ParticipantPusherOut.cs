using System.Collections;
using DG.Tweening;
using UnityEngine;

public class ParticipantPusherOut : ParticipantMover
{
    private float _pushDistanceKoef = 2;
    private float _angleRotation;
    private bool _isBot;
    private Vector3 _discardingDirection;
 
    public float RepulsionTime => MovingParamsController.TurnOverTime + MovingParamsController.PreparingPushTime + MovingParamsController.PushTime;

    private void Start()
    {
        base.Start();
        _isBot = GetComponent<Bot>() is null == false;
    }

    private void Update()
    {
        base.Update();

        if (_isBot)
            return;
        
        if (BorderChecker.IsOutField(transform.position, out TouchBorder touchBorder))
        {
            if (IsPushing == false)
            {
                DoRepulsion(MovingDirection, touchBorder);
            }
        }
    }

    public void DoRopeRepulsion(Vector3 moveDirection, TouchBorder touchBorder, bool isBot = false)
    {
        StartPosition = transform.position;
        _discardingDirection = BorderChecker.GetDiscardingDirection(moveDirection, StartPosition).normalized;
        _pushDistanceKoef = isBot ? _pushDistanceKoef * MovingParamsController.BotReduceKoef : _pushDistanceKoef;
        NewPosition = StartPosition + _discardingDirection * _pushDistanceKoef;
        _angleRotation = BorderChecker.GetTurnOverAngle(moveDirection, _discardingDirection, touchBorder);

        PushingOutSequence = DOTween.Sequence();
        PushingOutSequence.Append(transform
            .DOLocalRotate(transform.rotation.eulerAngles + new Vector3(0, _angleRotation, 0),
                MovingParamsController.TurnOverTime)
            .SetEase(Ease.Linear));
        PushingOutSequence.Insert(0, transform
            .DOMove(StartPosition - _discardingDirection * MovingParamsController.BackStepKoef, MovingParamsController.TurnOverTime)
            .SetEase(Ease.Linear));
        RotateBeforePushing(new Vector2(-MovingParamsController.AngleRotateBeforePushing, 0), PushingOutSequence);
        PushingOutSequence.Append(transform.DOMove(NewPosition, isBot ? MovingParamsController.PushTime * MovingParamsController.BotReduceKoef : MovingParamsController.PushTime)
            .SetEase(Ease.Flash));

        StartCoroutine(StartRunAnimation(MovingParamsController.TurnOverTime + MovingParamsController.PreparingPushTime, MovingParamsController.PushTime));
        BoostCoroutine = StartCoroutine(StartBoost(RepulsionTime));
        StartCoroutine(Reset(RepulsionTime + Time.deltaTime));
        IsRuling = false;
        IsPushing = true;
    }

    private void RotateBeforePushing(Vector2 angle, Sequence seq)
    {
        seq.Append(transform
            .DOLocalRotate(transform.rotation.eulerAngles + new Vector3(angle.x, _angleRotation, angle.y),
                MovingParamsController.PreparingPushTime / 2)
            .SetEase(Ease.Linear));

        seq.Append(transform
            .DOLocalRotate(transform.rotation.eulerAngles + new Vector3(0, _angleRotation, 0), MovingParamsController.PreparingPushTime / 2)
            .SetEase(Ease.Linear));
    }

    public void DoRepulsion(Vector3 moveDirection, TouchBorder touchBorder, bool isBot = false)
    {
        if (IsBoosting == false)
            StopBoost();
        
        DoRopeRepulsion(moveDirection, touchBorder);
    }
    
    private IEnumerator StartBoost(float delay)
    {
        Rigidbody.isKinematic = true;

        if (IsBoosting) 
            Speed /= MovingParamsController.Boost;

        if (BoostCoroutine != null)
        {
            Speed = StartSpeed;
            StopCoroutine(BoostCoroutine);
        }

        yield return new WaitForSeconds(delay);
        Participant.SetBoostEffectsVisibility(true);
        Speed *= MovingParamsController.Boost;
        IsBoosting = true;
        Rigidbody.isKinematic = true;
        Animator.SetFloat(AnimatorParticipantController.Params.Speed, Speed);
        yield return new WaitForSeconds(MovingParamsController.BoostTime);
        Speed = StartSpeed;
        IsBoosting = false;
        Rigidbody.isKinematic = true;
        Participant.SetBoostEffectsVisibility(false);
        Animator.SetFloat(AnimatorParticipantController.Params.Speed, Speed);
    }
    
    public void StopBoost()
    {
        if (BoostCoroutine != null)
            StopCoroutine(BoostCoroutine);

        Speed = StartSpeed;
        Animator.SetFloat(AnimatorParticipantController.Params.Speed, Speed);
        IsBoosting = false;
        IsRuling = true;
        IsPushing = false;
        Participant.SetBoostEffectsVisibility(false);
    }
}