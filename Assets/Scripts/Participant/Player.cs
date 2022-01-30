using UnityEngine;
using UnityEngine.Events;

public class Player : Participant
{
    [SerializeField] private int _startMoney;
    
    public int Money { get; private set; }
    public event UnityAction<int> MoneyAdded;

    private Vector3 _startPosition;
    
    private void OnEnable()
    {
        Money = _startMoney;
        _startPosition = transform.position;
    }

    public void AddMoney(int money)
    {
        Money += money;
        MoneyAdded?.Invoke(money);
    }

    public void ResetPlayer()
    {
        transform.position = _startPosition;
    }
}
