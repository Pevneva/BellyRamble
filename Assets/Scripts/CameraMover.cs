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
    private float _zoom;
    private float _offsetY;

    private void Start()
    {
        _isHorizontalTracking = true;
        _camera = GetComponent<Camera>();
        _camera.fieldOfView = 40;
        _zoom = 20;
        _offsetY = 16.5f;
    }

    private void Update()
    {
        if (_trackTarget == null)
            return; 

        _positionY = _isHorizontalTracking ? transform.position.y : _trackTarget.position.y + _offsetY;
        _positionZ = _isHorizontalTracking ? transform.position.z : _trackTarget.position.z - 8;
        _target = new Vector3(_trackTarget.position.x, _positionY, _positionZ);
        
        _currentPosition = Vector3.Lerp(transform.position, _target, _trackingSpeed * Time.deltaTime);
        transform.position = _currentPosition;
    }

    public void SetKindMoving(bool isHorizontal)
    {
        _isHorizontalTracking = isHorizontal;
    }

    public void SetPosition(Vector3 position)
    {
        _positionY = _isHorizontalTracking ? transform.position.y : position.y + _offsetY;
        _positionZ = _isHorizontalTracking ? transform.position.z : position.z - 8;
        transform.position = new Vector3(position.x, _positionY, _positionZ);        
    }

    public void SetTarget(Transform newTarget)
    {
        _trackTarget = newTarget;
    }

    public void ZoomIn()
    {
        _camera.fieldOfView -= _zoom;
    }

    public void ZoomOut()
    {
        _camera.DOFieldOfView(_camera.fieldOfView + _zoom, 1f).SetDelay(1);
    }
}