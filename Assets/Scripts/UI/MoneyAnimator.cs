using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

[RequireComponent(typeof(WinPanel))]
public class MoneyAnimator : MonoBehaviour
{
    [SerializeField] private Image _moneyPrefab;
    [SerializeField] private GameObject _moneyIconContainer;
    [SerializeField] private int _itemsAmount;
    [SerializeField] private Transform _creatingPoint;
    [SerializeField] private Transform _targetPoint;

    private ObjectPool _pool = new ObjectPool();
    private int _capacityPool = 20;
    private List<Image> _moneyIcons = new List<Image>();
    private Image _currentMoneyIcon;
    private float _moneyAnimationDelay;

    private void OnEnable()
    {
        _moneyAnimationDelay = GetComponent<WinPanel>().ShowingDuration;
        _pool.Initialize(_moneyPrefab, _moneyIconContainer, _capacityPool);
        // Invoke(nameof(CreateAndAnimateMoney), _moneyAnimationDelay);
    }

    public void CreateAndAnimateMoney(float countMoneyTime, float flyMoneyTime)
    {
        for (int i = 0; i < _itemsAmount; i++)
        {
            if (_pool.TryGetObject(out _currentMoneyIcon))
                SetMoneyIcon(_currentMoneyIcon, _moneyIconContainer, countMoneyTime, flyMoneyTime);

            Debug.Log("EEE _currentMoneyIcon : " + _currentMoneyIcon);
        }
    }

    private void SetMoneyIcon(Image image, GameObject parent, float countMoneyTime, float flyMoneyTime)
    {
        image.gameObject.SetActive(true);
        // image.gameObject.transform.parent = parent.transform;
        FlyMoney(image, countMoneyTime, flyMoneyTime);
    }

    private void FlyMoney(Image image, float countMoneyTime, float flyMoneyTime)
    {
        Vector2 startPosition = image.gameObject.transform.position;
        Debug.Log("RRR startPosition : " + startPosition);
        float randomX = Random.Range(startPosition.x - 250, startPosition.x + 251);
        float randomY = Random.Range(startPosition.y + 200, startPosition.y + 400);

        Sequence moving = DOTween.Sequence();
        moving.Append(
            _currentMoneyIcon.transform
                .DOMove(new Vector3(randomX, randomY, 0), countMoneyTime)
        );
        moving.Append(
            _currentMoneyIcon.transform.DOJump(_targetPoint.position, -5, 1, flyMoneyTime)
                .SetDelay(0.35f)
                .OnComplete(() =>
                {
                    Debug.Log("EEE COMPLETED !!!");
                    image.gameObject.SetActive(false);
                    image.gameObject.transform.parent = null;
                }));
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