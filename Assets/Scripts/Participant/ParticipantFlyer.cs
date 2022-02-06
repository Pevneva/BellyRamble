using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

public class ParticipantFlyer : MonoBehaviour
{
    public event UnityAction FlyStarted;
    
    private Camera _mainCamera;

    private void Start()
    {
        _mainCamera = Camera.main;
    }
    
    public void Fly(Vector3 direction, bool isCameraMoving)
    {
        Vector3 directionWithoutY = new Vector3(direction.x, 0, direction.z);
        if (isCameraMoving)
        {
            CameraMover cameraMover = _mainCamera.gameObject.GetComponent<CameraMover>();
            cameraMover.SetKindMoving(false);
            cameraMover.SetTarget(transform);
        }

        FlyStarted?.Invoke();

        var startPosition = transform.position;
        var heihgestPosition = startPosition + directionWithoutY.normalized * 8 + new Vector3(0, 5, 0);
        var endPosition = heihgestPosition + directionWithoutY.normalized * 16 + new Vector3(0, -10, 0);

        Sequence sequence = DOTween.Sequence();
        sequence.Append(transform.DOMove(heihgestPosition, MovingParamsController.FlyingTime / 3).SetEase(Ease.Linear));
        sequence.Append(transform.DOMove(endPosition, 2 * MovingParamsController.FlyingTime / 3).SetEase(Ease.Linear)
            .OnComplete(() => { }));
        StartCoroutine(CheckBottleEnded(MovingParamsController.FlyingTime));
    }
    
    private IEnumerator CheckBottleEnded(float delay)
    {
        yield return new WaitForSeconds(delay);
        // if (BattleController.IsBottleEnded() == false)
        Destroy(gameObject);
    }
}