using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

public class ParticipantFlyer : MonoBehaviour
{
    public event UnityAction FlyStarted;
    public event UnityAction<Transform> CameraFlyStarted; 
    
    private Camera _mainCamera;
    private BattleController _battleController;

    private void Start()
    {
        _mainCamera = Camera.main;
        _battleController = FindObjectOfType<BattleController>();
    }
    
    public void Fly(Vector3 direction, bool isCameraMoving)
    {
        Vector3 directionWithoutY = new Vector3(direction.x, 0, direction.z);
        if (isCameraMoving)
        {
            Debug.Log("AAA transform : " + transform + "; Time: " + Time.deltaTime);
            // CameraFlyStarted?.Invoke(transform); AAA
            _battleController.FlyingCameraStart(transform);
            // CameraMover cameraMover = _mainCamera.gameObject.GetComponent<CameraMover>();
            // cameraMover.SetKindMoving(false);
            // cameraMover.SetTarget(transform);
        }

        FlyStarted?.Invoke();

        var startPosition = transform.position;
        var heihgestPosition = startPosition + directionWithoutY.normalized * MovingParamsController.FlyingRangeKoef + 
                               new Vector3(0, MovingParamsController.FlyingHeight, 0);
        var endPosition = heihgestPosition + directionWithoutY.normalized * MovingParamsController.FlyingRangeKoef * 2 + 
                          new Vector3(0, -MovingParamsController.FlyingHeight * 2f, 0);

        Sequence sequence = DOTween.Sequence();
        sequence.Append(transform.DOMove(heihgestPosition, MovingParamsController.FlyingTime / 3).SetEase(Ease.Linear));
        sequence.Append(transform.DOMove(endPosition, 2 * MovingParamsController.FlyingTime / 3).SetEase(Ease.Linear));
        StartCoroutine(CheckBottleEnded(MovingParamsController.FlyingTime));
    }
    
    private IEnumerator CheckBottleEnded(float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(gameObject);
    }
}