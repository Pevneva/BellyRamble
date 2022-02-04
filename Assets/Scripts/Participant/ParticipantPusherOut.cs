using System.Collections;
using System.Collections.Generic;
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
    private float _angleRotation;
    private bool _isNotBot;
    private Vector3 _discardingDirection;


    private void Start()
    {
        base.Start();
        _isNotBot = GetComponent<Bot>() is null;
    }

    private void Update()
    {
        base.Update();

        if (_isNotBot == false)
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
        _discardingDirection = BorderChecker.GetDiscardingDirection(moveDirection, transform.position).normalized;

        StartPosition = transform.position;
        _pushDistanceKoef = isBot ? _pushDistanceKoef * 0.75f : _pushDistanceKoef;
        NewPosition = StartPosition + _discardingDirection * _pushDistanceKoef;

        TurnOverSequence = DOTween.Sequence();
        _angleRotation = BorderChecker.GetTurnOverAngle(moveDirection, _discardingDirection, touchBorder);

        TurnOverSequence.Append(transform
            .DOLocalRotate(transform.rotation.eulerAngles + new Vector3(0, _angleRotation, 0),
                _turnOverTime)
            .SetEase(Ease.Linear));
        TurnOverSequence.Insert(0, transform
            .DOMove(transform.position - _discardingDirection * 0.15f, _turnOverTime)
            .SetEase(Ease.Linear));
        RotateBeforePushing(new Vector2(-_angleRotateBeforePushing, 0), TurnOverSequence);
        TurnOverSequence.Append(transform.DOMove(NewPosition, isBot ? _pushTime * 0.75f : _pushTime)
            .SetEase(Ease.Flash));

        StartCoroutine(StartRunAnimation(_turnOverTime + _preparingPushTime, _pushTime));
        BoostCoroutine = StartCoroutine(StartBoost(_turnOverTime + _preparingPushTime + _pushTime));
        StartCoroutine(Reset(_turnOverTime + _preparingPushTime + _pushTime + 0.025f));
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
        {
            DoRopeRepulsion(moveDirection, touchBorder);
        }
        else
        {
            StopBoost();
            DoRopeRepulsion(moveDirection, touchBorder);
        }
    }
}