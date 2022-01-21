using DG.Tweening;
using UnityEngine;

public class CameraMover : MonoBehaviour
{
    [SerializeField] private Transform _trackTarget;
    [SerializeField] private float _trackingSpeed = 1.5f;

    private Vector3 _target;
    private Vector3 _currentPosition;
    private float _positionY;
    private float _positionZ;
    private bool _isHorizontalTracking;
    private Camera _camera;

    private void Start()
    {
        _isHorizontalTracking = true;
        _camera = GetComponent<Camera>();
    }

    private void Update()
    {
        if (_trackTarget == null)
            return; 

        _positionY = _isHorizontalTracking ? transform.position.y : _trackTarget.position.y + 16.5f;
        _positionZ = _isHorizontalTracking ? transform.position.z : _trackTarget.position.z - 8;
        _target = new Vector3(_trackTarget.position.x, _positionY, _positionZ);
        
        _currentPosition = Vector3.Lerp(transform.position, _target, _trackingSpeed * Time.deltaTime);
        transform.position = _currentPosition;
    }

    public void SetKindMoving(bool isHorizontal)
    {
        _isHorizontalTracking = isHorizontal;
    }

    public void SetTarget(Transform newTarget)
    {
        _trackTarget = newTarget;
    }

    private void ZoomIn()
    {
        _camera.fieldOfView -= 30;
    }

    private void ZoomOut()
    {
        _camera.DOFieldOfView(_camera.fieldOfView + 30, 1.5f);
    }
}