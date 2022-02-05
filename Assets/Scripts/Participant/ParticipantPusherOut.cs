using System.Collections;
using DG.Tweening;
using UnityEngine;

public class ParticipantPusherOut : ParticipantMover
{
    public float RepulsionTime => _turnOverTime + _preparingPushTime + _pushTime;

    private readonly float _turnOverTime = 0.2f;
    private readonly float _preparingPushTime = 0.3f;
    private readonly float _angleRotateBeforePushing = 15;
    private readonly float _pushTime = 0.2f;
    private float _pushDistanceKoef = 2;
    private float _botReduceKoef = 0.75f;
    private float _backStepKoef = 0.15f;
    private float _angleRotation;
    private bool _isBot;
    private Vector3 _discardingDirection;
    
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
        _pushDistanceKoef = isBot ? _pushDistanceKoef * _botReduceKoef : _pushDistanceKoef;
        NewPosition = StartPosition + _discardingDirection * _pushDistanceKoef;
        _angleRotation = BorderChecker.GetTurnOverAngle(moveDirection, _discardingDirection, touchBorder);

        TurnOverSequence = DOTween.Sequence();
        TurnOverSequence.Append(transform
            .DOLocalRotate(transform.rotation.eulerAngles + new Vector3(0, _angleRotation, 0),
                _turnOverTime)
            .SetEase(Ease.Linear));
        TurnOverSequence.Insert(0, transform
            .DOMove(StartPosition - _discardingDirection * _backStepKoef, _turnOverTime)
            .SetEase(Ease.Linear));
        RotateBeforePushing(new Vector2(-_angleRotateBeforePushing, 0), TurnOverSequence);
        TurnOverSequence.Append(transform.DOMove(NewPosition, isBot ? _pushTime * _botReduceKoef : _pushTime)
            .SetEase(Ease.Flash));

        StartCoroutine(StartRunAnimation(_turnOverTime + _preparingPushTime, _pushTime));
        BoostCoroutine = StartCoroutine(StartBoost(_turnOverTime + _preparingPushTime + _pushTime));
        StartCoroutine(Reset(_turnOverTime + _preparingPushTime + _pushTime + Time.deltaTime));
        IsRuling = false;
        IsPushing = true;
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
            Speed /= Boost;

        if (BoostCoroutine != null)
        {
            Speed = StartSpeed;
            StopCoroutine(BoostCoroutine);
        }

        yield return new WaitForSeconds(delay);
        Participant.SetBoostEffectsVisibility(true);
        Speed *= Boost;
        IsBoosting = true;
        Rigidbody.isKinematic = true;
        Animator.SetFloat(AnimatorParticipantController.Params.Speed, Speed);
        yield return new WaitForSeconds(BoostTime);
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