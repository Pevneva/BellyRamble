using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(MoneyAnimator))]
public class WinPanel : MonoBehaviour
{
    [SerializeField] private Button _getMoneyButton;
    [SerializeField] private Button _adsGetMoneyButton;
    
    [SerializeField] private Image _getMoneyButtonImage;
    [SerializeField] private Image _getMoneyOutlinenImage;
    [SerializeField] private TMP_Text _getMoneyText;
    [SerializeField] private Image _getMoneyButtonMoneyIcon;
    [SerializeField] private float _showingDuration;

    public float ShowingDuration => _showingDuration;
    
    private MoneyAnimator _moneyAnimator;
    private bool _isResetMoney;
    private float _rewardValue;
    private float _countMoneyTime;
    private float _flyMoneyTime;
    private float _changeMoneyStep;
    

    private void OnEnable()
    {
        _isResetMoney = false;
        _rewardValue = 90;
        _getMoneyText.text = _rewardValue.ToString();
        _countMoneyTime = 0.75f;
        _flyMoneyTime = 1;
        _changeMoneyStep = _rewardValue / _countMoneyTime;
        
        _moneyAnimator = GetComponent<MoneyAnimator>();
        _getMoneyButton.onClick.AddListener(GetMoney);
        _adsGetMoneyButton.onClick.AddListener(ShowGetMoneyButton);
        _showingDuration = 3;
        Debug.Log("EEE OnEnable WinPanel");
        ShowGetMoneyButton();
        
    }

    private void Update()
    {
        if (_isResetMoney)
        {
            _rewardValue -= _changeMoneyStep * Time.deltaTime;
            if (_rewardValue <= 0)
            {
                _getMoneyText.text = 0.ToString();
                _isResetMoney = false;
                return;
            }
            
            Debug.Log("RRR _rewardValue : " + Mathf.Round(_rewardValue));
            _getMoneyText.text = Mathf.Round(_rewardValue).ToString();

        }
            
    }

    private void GetMoney()
    {
        _moneyAnimator.CreateAndAnimateMoney(_countMoneyTime, _flyMoneyTime); 
        _getMoneyButton.onClick.RemoveListener(GetMoney);
        _isResetMoney = true;
    }

    private void ResetMoney()
    {
        
    } 

    private void ShowGetMoneyButton()
    {
        Debug.Log("EEE ShowGetMoneyButton!");
        Debug.Log("EEE ShowGetMoneyButton! _showingDuration : "+ _showingDuration);
        _getMoneyButtonImage.DOFade(0, 0);
        _getMoneyOutlinenImage.DOFade(0, 0);
        _getMoneyText.DOFade(0, 0);
        _getMoneyButtonMoneyIcon.DOFade(0, 0);
        
        Debug.Log("EEE ShowGetMoneyButton 2 " + _getMoneyButtonImage.color.grayscale);
        _getMoneyButtonImage.DOFade(1, _showingDuration);
        _getMoneyOutlinenImage.DOFade(1, _showingDuration);
        _getMoneyText.DOFade(1, _showingDuration);
        _getMoneyButtonMoneyIcon.DOFade(1, _showingDuration);
        
        // _getMoneyOutlinenImage.gameObject.transform.DOMoveY(_getMoneyOutlinenImage.gameObject.transform.position.y + 100, 3).SetDelay(2);
    }
}
