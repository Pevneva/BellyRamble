using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class MoneyAnimator : MonoBehaviour
{
    [SerializeField] private Image _moneyPrefab;
    [SerializeField] private GameObject _moneyIconContainer;
    [SerializeField] private int _itemsAmount;
    [SerializeField] private float _countMoneyTime;
    [SerializeField] private float _flyMoneyTime;
    [SerializeField] private float _flyInMoneyTime;

    public float CountMoneyTime => _countMoneyTime;
    public float FlyInMoneyTime => _flyInMoneyTime;
    public float BeforeFlyInMoneyTime => _flyMoneyTime + CountMoneyTime;

    private Transform _targetPoint;
    private Transform _beforeFlyInPoint;
    private ObjectPool _pool = new ObjectPool();
    private int _capacityPool = 20;
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
        _countMoneyTime = 0.35f;
        _flyMoneyTime = 1;
        _flyInMoneyTime = 0.55f;
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
        image.transform.localScale *= 1.1f;
        FlyMoney(image, countMoneyTime, flyMoneyTime);
    }

    private void FlyMoney(Image image, float countMoneyTime, float flyMoneyTime)
    {
        float randomX = Random.Range(- 200, 125);
        float randomY = Random.Range ( 200, 350);

        Sequence moving = DOTween.Sequence();
        moving.Append(
            _currentMoneyIcon.transform
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
                    image.gameObject.SetActive(false);
                    image.gameObject.transform.parent = null;
                })
        );
    }
}