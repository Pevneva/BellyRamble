using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MoneyView : MonoBehaviour
{
    [SerializeField] private Player _player;
    [SerializeField] private TMP_Text _moneyText;

    private MoneyAnimator _moneyAnimator;
    private float _countTime;
    private bool _isCounting;
    private float _addedMoney;
    private float _tempAddedMoney;
    private int _startMoney;

    private void Start()
    {
        _tempAddedMoney = 0;
        _isCounting = false;
        _moneyAnimator = FindObjectOfType<MoneyAnimator>();
        _startMoney = _player.Money;
        _moneyText.text = _player.Money.ToString();
        _countTime = _moneyAnimator.FlyInMoneyTime;
        _player.MoneyAdded += OnMoneyAdded;
    }

    private void OnDisable()
    {
        _player.MoneyAdded -= OnMoneyAdded;
    }

    private void Update()
    {
        TryCount();
    }

    private void OnMoneyAdded(int addedMoney)
    {
        _startMoney = _player.Money - addedMoney;
        _addedMoney = addedMoney;

        Invoke(nameof(StartCounting), _moneyAnimator.BeforeFlyInMoneyTime);
    }

    private void StartCounting()
    {
        _isCounting = true;
    }

    private void TryCount()
    {
        if (_isCounting)
        {
            _tempAddedMoney +=  _addedMoney / (_countTime * 2 - Time.deltaTime)  * Time.deltaTime;
            
            if (_tempAddedMoney >= _addedMoney)
            {
                _moneyText.text = _player.Money.ToString();
                _isCounting = false;
                return;
            }
            
            _moneyText.text = (Mathf.Round(_tempAddedMoney) + _startMoney).ToString();
        }
    }
}