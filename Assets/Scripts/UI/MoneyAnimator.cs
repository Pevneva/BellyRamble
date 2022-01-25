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
    [FormerlySerializedAs("_countMouneyTime")] [SerializeField] private float _countMoneyTime;
    [SerializeField] private float _flyMoneyTime;
    [SerializeField] private float _flyInMoneyTime;

    public float CountMoneyTime => _countMoneyTime;
    // public float CountMoneyTime { get; private set; }
    public float FlyInMoneyTime => _flyInMoneyTime;
    // public float FlyInMoneyTime { get; private set; }
    public float BeforeFlyInMoneyTime
    {
        get { return _flyMoneyTime + CountMoneyTime; }
    }

    private Transform _targetPoint;
    private Transform _beforeFlyInPoint;
    private ObjectPool _pool = new ObjectPool();
    private int _capacityPool = 20;
    private Image _currentMoneyIcon;
    private float _countingPlayerTimeDelayStep;
    private static float s_delayPlayerTimeItem;

    private void Start()
    {
        // CountMoneyTime = 0.35f;
        // _flyMoneyTime = 1;
        // FlyInMoneyTime = 0.55f;
        s_delayPlayerTimeItem = 0;
        // _flyingTime = _flyMoneyTime + CountMoneyTime;
        Debug.Log("==============================================================================");
        Debug.Log("SEE: Start MoneyAnimator; FlyInMoneyTime : " + FlyInMoneyTime + "; BeforeFlyInMoneyTime : " + BeforeFlyInMoneyTime);
        Debug.Log("SEE: Start CountMoneyTime : " + CountMoneyTime + "; _flyMoneyTime : " + _flyMoneyTime);
        Debug.Log("==============================================================================");
        _countingPlayerTimeDelayStep = FlyInMoneyTime / _itemsAmount;
    }
    //
    // private void Start()
    // {
    //     throw new NotImplementedException();
    // }

    public void InitializePool()
    {
        _pool.Initialize(_moneyPrefab, _moneyIconContainer, _capacityPool);
    }

    public void InitFlyingData()
    {
        _countMoneyTime = 0.35f;
        _flyMoneyTime = 1;
        _flyInMoneyTime = 0.55f;
        _countingPlayerTimeDelayStep = FlyInMoneyTime / _itemsAmount;
        Debug.Log("SEE InitFlyingData CountMoneyTime : " + CountMoneyTime);
        Debug.Log("SEE InitFlyingData CountMoneyTime : " + FlyInMoneyTime);
        Debug.Log("SEE InitFlyingData _flyMoneyTime : " + _flyMoneyTime);
        Debug.Log("SEE InitFlyingData _countingPlayerTimeDelayStep : " + _countingPlayerTimeDelayStep);
    }

    public void SetTargets(Transform target, Transform beforeFlyIn)
    {
        _targetPoint = target;
        _beforeFlyInPoint = beforeFlyIn;
        Debug.Log("SEE: SetTargets, MoneyAnimator; _targetPoint " + _targetPoint + "; _beforeFlyInPoint : " + _beforeFlyInPoint);
    }

    public void CreateAndAnimateMoney()
    {
        Debug.Log("SEE CreateAndAnimateMoney CountMoneyTime : " + CountMoneyTime);
        Debug.Log("SEE CreateAndAnimateMoney CountMoneyTime : " + FlyInMoneyTime);
        Debug.Log("SEE CreateAndAnimateMoney _flyMoneyTime : " + _flyMoneyTime);
        
        for (int i = 0; i < _itemsAmount; i++)
        {
            if (_pool.TryGetObject(out _currentMoneyIcon))
                SetMoneyIcon(_currentMoneyIcon, _moneyIconContainer, CountMoneyTime, _flyMoneyTime);

            Debug.Log("EEE _currentMoneyIcon : " + _currentMoneyIcon);
        }
    }

    private void SetMoneyIcon(Image image, GameObject parent, float countMoneyTime, float flyMoneyTime)
    {
        Debug.Log("SEE SetMoneyIcon countMoneyTime : " + countMoneyTime);
        Debug.Log("SEE SetMoneyIcon flyMoneyTime : " + flyMoneyTime);
        image.gameObject.SetActive(true);
        image.transform.localScale *= 1.1f;
        Debug.Log("SEE image : " + image);
        // image.gameObject.transform.parent = parent.transform;
        FlyMoney(image, countMoneyTime, flyMoneyTime);
    }

    private void FlyMoney(Image image, float countMoneyTime, float flyMoneyTime)
    {
        // Vector2 startPosition = image.gameObject.transform.position;
        Vector2 startPosition = Vector2.zero;

        Debug.Log("SEE CountMoneyTime : " + CountMoneyTime);
        Debug.Log("SEE _flyMoneyTime : " + _flyMoneyTime);
        Debug.Log("MONEY_PLAYER startPosition : " + startPosition);
        // float randomX = Random.Range(startPosition.x - 200, startPosition.x + 125);
        // float randomY = Random.Range(startPosition.y + 200, startPosition.y + 350);
        float randomX = Random.Range(- 200, 125);
        float randomY = Random.Range ( 200, 350);
        // float randomX = startPosition.x - 200;
        // float randomY = startPosition.y + 200;
        
        Debug.Log("SEE startPosition.x : " + startPosition.x);
        Debug.Log("SEE startPosition.y : " + startPosition.y);        
        Debug.Log("SEE randomX : " + randomX);
        Debug.Log("SEE randomY : " + randomY);
        Debug.Log("SEE _flyMoneyTime : " + _flyMoneyTime);

        Sequence moving = DOTween.Sequence();
        moving.Append(
            _currentMoneyIcon.transform
                // .DOLocalMove(new Vector3(-100, 200, 0), CountMoneyTime)
                .DOLocalMove(new Vector3(randomX, randomY, 0), CountMoneyTime)
                .SetEase(Ease.Flash)
            
        );
        
        s_delayPlayerTimeItem += _countingPlayerTimeDelayStep;
        
        moving.Append(
            _currentMoneyIcon.transform.DOMove(_beforeFlyInPoint.position, _flyMoneyTime)
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
}