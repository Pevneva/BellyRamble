using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

public class ParticipantFlyer : MonoBehaviour
{
    private BattleController _battleController;
    
    public event UnityAction FlyStarted;

    private void Start()
    {
        _battleController = FindObjectOfType<BattleController>();
    }
    
    public void Fly(Vector3 direction, bool isCameraMoving)
    {
        Vector3 directionWithoutY = new Vector3(direction.x, 0, direction.z);
        if (isCameraMoving)
            _battleController.FlyingCameraStart(transform);

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