using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BorderChecker : MonoBehaviour
{
    [SerializeField] private Transform _leftDownShpere;
    [SerializeField] private Transform _rightUpShpere;
    [SerializeField] private Transform _leftDownPilliar;
    [SerializeField] private Transform _leftUpPilliar;
    [SerializeField] private Transform _rightUpPilliar;
    [SerializeField] private Transform _rightDownPilliar;
    
    private readonly float _offset = 0.35f;
    private readonly float _offsetToPush = -0.1f;
    private float _radius;
    private Vector2 _planeStartPoint;

    public Vector2 CenterPositionXZ { get; private set; }

    private void Start()
    {
        CenterPositionXZ = GetRingCenter();
        _planeStartPoint = new Vector2(_leftDownShpere.position.x + _offset,
            _leftDownShpere.position.z + _offset);
        _radius = Vector2.Distance(CenterPositionXZ, _planeStartPoint);
    }

    public Vector2 GetRingCenter()
    {
        Vector2 startPosition = new Vector2(_leftDownShpere.position.x, _leftDownShpere.position.z);
        Vector2 endPosition = new Vector2(_rightUpShpere.position.x, _rightUpShpere.position.z);

        return new Vector2((endPosition.x - startPosition.x) / 2 + startPosition.x,
            (endPosition.y - startPosition.y) / 2 + startPosition.y);
    }

    public bool IsOutsideRing(Vector2 positionXZ)
    {
        return Vector2.Distance(CenterPositionXZ, positionXZ) > _radius - _offset;
    }

    public bool IsOutField(Vector3 position, out TouchBorder touchBorder)
    {
        if (position.x < _leftDownShpere.position.x - _offsetToPush)
        {
            touchBorder = TouchBorder.LEFT;
            return true;
        }

        if (position.x > _rightUpShpere.position.x + _offsetToPush)
        {
            touchBorder = TouchBorder.RIGHT;
            return true;
        }

        if (position.z < _leftDownShpere.position.z - _offsetToPush)
        {
            touchBorder = TouchBorder.DOWN;
            return true;
        }

        if (position.z > _rightUpShpere.position.z + _offsetToPush)
        {
            touchBorder = TouchBorder.UP;
            return true;
        }

        touchBorder = TouchBorder.NULL;
        return false;
    }

    public float GetTurnOverAngle(Vector3 moveDirection, Vector3 discardingDirection, TouchBorder touchBorder)
    {
        if (touchBorder == TouchBorder.LEFT && moveDirection.z > 0)
            return 2 * Vector3.Angle(Vector3.forward, discardingDirection);

        if (touchBorder == TouchBorder.LEFT && moveDirection.z < 0)
            return -2 * Vector3.Angle(Vector3.back, discardingDirection);

        if (touchBorder == TouchBorder.RIGHT && moveDirection.z > 0)
            return -2 * Vector3.Angle(Vector3.forward, discardingDirection);

        if (touchBorder == TouchBorder.RIGHT && moveDirection.z < 0)
            return 2 * Vector3.Angle(Vector3.back, discardingDirection);

        if (touchBorder == TouchBorder.UP && moveDirection.x > 0)
            return 2 * Vector3.Angle(Vector3.right, discardingDirection);

        if (touchBorder == TouchBorder.UP && moveDirection.x < 0)
            return -2 * Vector3.Angle(Vector3.left, discardingDirection);

        if (touchBorder == TouchBorder.DOWN && moveDirection.x > 0)
            return -2 * Vector3.Angle(Vector3.right, discardingDirection);

        if (touchBorder == TouchBorder.DOWN && moveDirection.x < 0)
            return 2 * Vector3.Angle(Vector3.left, discardingDirection);

        return 0;
    }
    
    public Vector3 GetDiscardingDirection(Vector3 direction, Vector3 position)
    {
        float positionX = position.x;
        float positionZ = position.z;

        if (positionX < _leftDownShpere.position.x + 0.65f || positionX > _rightUpShpere.position.x - 0.65f)
            return new Vector3(-direction.x, direction.y, direction.z);

        if (positionZ < _leftDownShpere.position.z + 0.65f || positionZ > _rightUpShpere.position.z - 0.65f)
            return new Vector3(direction.x, direction.y, -direction.z);

        return Vector3.zero;
    }
    
    public bool IsAngleNearby(Vector3 position, float distance)
    {
        if (Vector3.Distance(position, _leftDownPilliar.position) < distance)
            return true;

        if (Vector3.Distance(position, _leftUpPilliar.position) < distance)
            return true;

        if (Vector3.Distance(position, _rightUpPilliar.position) < distance)
            return true;

        if (Vector3.Distance(position, _rightDownPilliar.position) < distance)
            return true;

        return false;
    }
}