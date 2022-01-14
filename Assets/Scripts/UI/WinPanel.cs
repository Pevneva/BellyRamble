using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WinPanel : MonoBehaviour
{
    [SerializeField] private Image _getMoneyButtonImage;
    [SerializeField] private Image _getMoneyOutlinenImage;
    [SerializeField] private TMP_Text _getMoneyText;
    [SerializeField] private Image _getMoneyButtonMoneyIcon;
    [SerializeField] private float _showingDuration;

    private void OnEnable()
    {
        _showingDuration = 4;
        Debug.Log("EEE OnEnable WinPanel");
        ShowGetMoneyButton();
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
    }
}
