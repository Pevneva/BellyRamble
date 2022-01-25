using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;

public class Player : Participant
{
    [SerializeField] private int _startMoney;
    
    public int Money { get; private set; }
    public event UnityAction<int> MoneyAdded;

    private Vector3 _startPosition;
    
    private void OnEnable()
    {
        // base.Start();
        Money = _startMoney;
        _startPosition = transform.position;
        // MoneyAdded?.Invoke(Money);
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
