using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class MoneyAnimator : MonoBehaviour
{
    [SerializeField] private GameObject _moneyPrefab;
    [SerializeField] private Transform _creatingPoint;
    [SerializeField] private Transform _targetPoint;
    [SerializeField] private int _itemsAmount;

    private void OnEnable()
    {
        CreateAndAnimateMoney();
    }

    private void CreateAndAnimateMoney()
    {
        for (int i = 0; i < _itemsAmount; i++)
        {
            Instantiate(_moneyPrefab, _creatingPoint);
        }
    }
}