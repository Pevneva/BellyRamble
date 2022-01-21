using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class MoneyAnimator : MonoBehaviour
{
    [SerializeField] private Image _moneyPrefab;
    [SerializeField] private GameObject _moneyIconContainer;
    [SerializeField] private int _itemsAmount;

    public float CountMoneyTime { get; private set; }
    public float FlyInMoneyTime { get; private set; }

    public float BeforeFlyInMoneyTime
    {
        get { return _flyMoneyTime + CountMoneyTime; }
    }

    private Transform _targetPoint;
    private Transform _beforeFlyInPoint;
    private float _flyMoneyTime;
    private ObjectPool _pool = new ObjectPool();
    private int _capacityPool = 20;
    private Image _currentMoneyIcon;
    private float _countingPlayerTimeDelayStep;
    private static float s_delayPlayerTimeItem;
    // private float _flyingTime;

    private void OnEnable()
    {
        CountMoneyTime = 0.35f;
        _flyMoneyTime = 1;
        FlyInMoneyTime = 0.55f;
        s_delayPlayerTimeItem = 0;
        // _flyingTime = _flyMoneyTime + CountMoneyTime;
        Debug.Log("SEE: OnEnable MoneyAnimator; FlyInMoneyTime : " + FlyInMoneyTime + "; BeforeFlyInMoneyTime : " + BeforeFlyInMoneyTime);
        _countingPlayerTimeDelayStep = FlyInMoneyTime / _itemsAmount;
    }

    public void InitializePool()
    {
        _pool.Initialize(_moneyPrefab, _moneyIconContainer, _capacityPool);
    }

    public void SetTargets(Transform target, Transform beforeFlyIn)
    {
        _targetPoint = target;
        _beforeFlyInPoint = beforeFlyIn;
        Debug.Log("SEE: SetTargets, MoneyAnimator; _targetPoint " + _targetPoint + "; _beforeFlyInPoint : " + _beforeFlyInPoint);
    }

    public void CreateAndAnimateMoney()
    {
        for (int i = 0; i < _itemsAmount; i++)
        {
            if (_pool.TryGetObject(out _currentMoneyIcon))
                SetMoneyIcon(_currentMoneyIcon, _moneyIconContainer, CountMoneyTime, _flyMoneyTime);

            Debug.Log("EEE _currentMoneyIcon : " + _currentMoneyIcon);
        }
    }

    private void SetMoneyIcon(Image image, GameObject parent, float countMoneyTime, float flyMoneyTime)
    {
        image.gameObject.SetActive(true);
        image.transform.localScale *= 1.1f;
        // image.gameObject.transform.parent = parent.transform;
        FlyMoney(image, countMoneyTime, flyMoneyTime);
    }

    private void FlyMoney(Image image, float countMoneyTime, float flyMoneyTime)
    {
        // _flyingTime = countMoneyTime + flyMoneyTime;

        Vector2 startPosition = image.gameObject.transform.position;

        Debug.Log("SEE countMoneyTime : " + countMoneyTime);
        Debug.Log("SEE flyMoneyTime : " + flyMoneyTime);
        Debug.Log("MONEY_PLAYER SEE startPosition : " + startPosition);
        float randomX = Random.Range(startPosition.x - 200, startPosition.x + 125);
        float randomY = Random.Range(startPosition.y + 200, startPosition.y + 350);
        // float randomX = startPosition.x - 200;
        // float randomY = startPosition.y + 200;

        Sequence moving = DOTween.Sequence();
        moving.Append(
            _currentMoneyIcon.transform
                .DOMove(new Vector3(randomX, randomY, 0), countMoneyTime)
                .SetEase(Ease.Flash)
        );
        s_delayPlayerTimeItem += _countingPlayerTimeDelayStep;


        moving.Append(
            _currentMoneyIcon.transform.DOMove(_beforeFlyInPoint.position, flyMoneyTime)
                .SetDelay(0.35f)
                .SetEase(Ease.Linear)
        );

        moving.Insert(countMoneyTime + flyMoneyTime - 0.15f,
            _currentMoneyIcon.transform
                .DOMove(_targetPoint.position, FlyInMoneyTime + s_delayPlayerTimeItem)
                .SetEase(Ease.Linear)
                .OnComplete(() =>
                {
                    Debug.Log("SEE COMPLETED !!! s_delayPlayerTimeItem : " + s_delayPlayerTimeItem);
                    image.gameObject.SetActive(false);
                    image.gameObject.transform.parent = null;
                })
        );
    }

    private void FlyToPoint(Image item, Transform targetObj, float time, int index, long amount, float delay)
    {
        Vector3 target = targetObj.transform.position;

        var scaleSequence = DOTween.Sequence();

        scaleSequence.Append(item.transform.DOScale(0.7f, 0));

        scaleSequence.Append(item.transform.DOScale(Random.Range(1.5f, 2f), 0.75f * time)
            .SetDelay(time / amount * index + delay));
        scaleSequence.Append(item.transform.DOScale(0.2f, 0.25f * time).SetDelay(time / amount * index + delay));

        // Context.DelayedCallback.Invoke(time - 0.2f, () =>
        // {
        //     item.transform.SetParent(targetObj.parent);
        //     item.transform.SetSiblingIndex(targetObj.GetSiblingIndex() - 1);
        // });
        //
        // item.transform.DOJump(target, -5, 1, time).SetDelay(time / amount * index + delay).OnComplete(() =>
        // {
        //     if(_callBacks.ContainsKey(item.transform))
        //         _callBacks[item.transform]?.Invoke();
        //     _objectsPool.Free(item);
        // });
    }
}