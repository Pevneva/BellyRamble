using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class MoneyAnimator : MonoBehaviour
{
    [SerializeField] private Image _moneyPrefab;
    [SerializeField] private GameObject _moneyIconContainer;
    [SerializeField] private int _itemsAmount;
    
    public float CountMoneyTime { get; } = 0.35f;
    public float FlyInMoneyTime { get; } = 0.55f;
    public float BeforeFlyInMoneyTime => _flyMoneyTime + CountMoneyTime;

    private readonly int _capacityPool = 20;
    private readonly float _flyMoneyTime = 1;
    private readonly float _delayBeforeCountingMoney = 0.35f;
    private readonly float _offsetTimeBeforeFlyIn = 0.15f;
    private readonly float _scaleIconKoef = 1.1f;
    private readonly float _spreadMoneyXMin = -200;
    private readonly float _spreadMoneyXMax = 125;
    private readonly float _spreadMoneyYMin = 200;
    private readonly float _spreadMoneyYMax = 350;
    private Transform _targetPoint;
    private Transform _beforeFlyInPoint;
    private ObjectPool _pool = new ObjectPool();
    private Image _currentMoneyIcon;
    private float _countingPlayerTimeDelayStep;
    private static float s_delayPlayerTimeItem;

    private void Start()
    {
        s_delayPlayerTimeItem = 0;
        _countingPlayerTimeDelayStep = FlyInMoneyTime / _itemsAmount;
    }

    public void InitializePool()
    {
        _pool.Initialize(_moneyPrefab, _moneyIconContainer, _capacityPool);
    }

    public void InitFlyingData()
    {
        _countingPlayerTimeDelayStep = FlyInMoneyTime / _itemsAmount;
    }

    public void SetTargets(Transform target, Transform beforeFlyIn)
    {
        _targetPoint = target;
        _beforeFlyInPoint = beforeFlyIn;
    }

    public void CreateAndAnimateMoney()
    {
        for (int i = 0; i < _itemsAmount; i++)
        {
            if (_pool.TryGetObject(out _currentMoneyIcon))
                SetMoneyIcon(_currentMoneyIcon, _moneyIconContainer, CountMoneyTime, _flyMoneyTime);
        }
    }

    private void SetMoneyIcon(Image image, GameObject parent, float countMoneyTime, float flyMoneyTime)
    {
        image.gameObject.SetActive(true);
        image.transform.localScale *= _scaleIconKoef;
        FlyMoney(image, countMoneyTime, flyMoneyTime);
    }

    private void FlyMoney(Image image, float countMoneyTime, float flyMoneyTime)
    {
        float randomX = Random.Range(_spreadMoneyXMin, _spreadMoneyXMax);
        float randomY = Random.Range (_spreadMoneyYMin, _spreadMoneyYMax);

        Sequence moving = DOTween.Sequence();
        moving.Append(
            _currentMoneyIcon.transform
                .DOLocalMove(new Vector3(randomX, randomY, 0), CountMoneyTime)
                .SetEase(Ease.Flash)
        );
        
        s_delayPlayerTimeItem += _countingPlayerTimeDelayStep;
        
        moving.Append(
            _currentMoneyIcon.transform.DOMove(_beforeFlyInPoint.position, _flyMoneyTime)
                .SetDelay(_delayBeforeCountingMoney)
                .SetEase(Ease.Linear)
        );
        
        moving.Insert(countMoneyTime + flyMoneyTime - _offsetTimeBeforeFlyIn,
            _currentMoneyIcon.transform
                .DOMove(_targetPoint.position, FlyInMoneyTime + s_delayPlayerTimeItem)
                .SetEase(Ease.Linear)
                .OnComplete(() =>
                {
                    image.gameObject.SetActive(false);
                    image.gameObject.transform.parent = _moneyIconContainer.transform;
                })
        );
    }
}