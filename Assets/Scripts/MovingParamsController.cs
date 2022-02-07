using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MovingParamsController
{
    public const float FlyingTime = 2.3f;
    public const float FlyingHeight = 5f;
    public const float FlyingRangeKoef = 10f;
    public const float TurnOverTime = 0.2f;
    public const float PreparingPushTime = 0.3f;
    public const float AngleRotateBeforePushing = 15;
    public const float PushTime = 0.2f;
    public const float BotReduceKoef = 0.75f;
    public const float BackStepKoef = 0.15f;
    public const float Boost = 2.4f;
    public const float BoostTime = 1.15f;
    public const float RopePointDistanceKoef = 1.25f;
    public const float IgnoredDistanceToAngle = 2f;
    public const float ProcessedDistanceToRope = 1.35f;
}