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
    private float _countingMoneyTimeStepPerFrame;
    private int _startMoney;

    private void Start()
    {
        _tempAddedMoney = 0;
        _moneyAnimator = FindObjectOfType<MoneyAnimator>();
        _startMoney = _player.Money;
        _moneyText.text = _player.Money.ToString();
        Debug.Log("MONEY_PLAYER SEE: _moneyAnimator :" + _moneyAnimator);
        Debug.Log("MONEY_PLAYER SEE: FlyInMoneyTime :" + _moneyAnimator.FlyInMoneyTime);
        _countTime = _moneyAnimator.FlyInMoneyTime;
        _isCounting = false;

        _player.MoneyAdded += OnMoneyAdded;
    }

    private void OnDisable()
    {
        _player.MoneyAdded -= OnMoneyAdded;
    }

    private void Update()
    {
        TryStartCounter();
    }

    private void OnMoneyAdded(int addedMoney)
    {
        _startMoney = _player.Money - addedMoney;
        _addedMoney = addedMoney;
        // _countingMoneyTimeStepPerFrame = _addedMoney / _countTime * Time.deltaTime;
        
        Debug.Log("[MONEY_PLAYER] _startMoney : " + _startMoney);
        Debug.Log("[MONEY_PLAYER] _addedMoney : " + _addedMoney);
        Debug.Log("[MONEY_PLAYER] _countTime : " + _countTime);
        
        Invoke(nameof(StartCounting), _moneyAnimator.BeforeFlyInMoneyTime);
    }

    private void StartCounting()
    {
        _isCounting = true;
    }

    private void TryStartCounter()
    {
        if (_isCounting)
        {
            _tempAddedMoney +=  _addedMoney / (_countTime * 2 - 0.25f)  * Time.deltaTime;
            
            
            if (_tempAddedMoney >= _addedMoney)
            {
                _moneyText.text = _player.Money.ToString();
                _isCounting = false;
                return;
            }

            // Debug.Log("[MONEY_PLAYER] addedMoney : " + _addedMoney);
            
            Debug.Log("[MONEY_PLAYER_CHECK] _startMoney : " + _startMoney);
            Debug.Log("[MONEY_PLAYER_CHECK_TEMP] tempAddedMoney : " + _tempAddedMoney);
            Debug.Log("[MONEY_PLAYER_CHECK] addedMoney / countTime * Time.deltaTime : " + (_addedMoney / _countTime * Time.deltaTime));
            _moneyText.text = (Mathf.Round(_tempAddedMoney) + _startMoney).ToString();
            
            // _moneyText.text = (Mathf.Round(_tempAddedMoney) + _countingMoneyTimeStepPerFrame * _countTime).ToString();
        }
    }
}