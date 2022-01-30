using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class WinPanel : MonoBehaviour
{
    [SerializeField] private Button _getMoneyButton;
    [SerializeField] private Button _adsGetMoneyButton;
    [SerializeField] private CanvasGroup _getMoneyButtonGroup;
    [SerializeField] private float _showingDuration;
    [SerializeField] private TMP_Text _getMoneyText;
    [SerializeField] private Transform _targetPoint;
    [SerializeField] private Transform _beforeFlyInPoint;

    public event UnityAction GetMoneyButtonPressed;
    
    private MoneyAnimator _moneyAnimator;
    private bool _isResetMoney;
    private float _rewardValue;
    private float _changeMoneyStep;
    private Player _player;

    private void Start()
    {
        _isResetMoney = false;
        _rewardValue = 90;
        _getMoneyText.text = _rewardValue.ToString();
        _moneyAnimator = FindObjectOfType<MoneyAnimator>();
        _moneyAnimator.SetTargets(_targetPoint, _beforeFlyInPoint);
        _moneyAnimator.InitFlyingData();
        _changeMoneyStep = _rewardValue / _moneyAnimator.CountMoneyTime;
        _moneyAnimator.InitializePool();
        _moneyAnimator.InitFlyingData();
        _player = FindObjectOfType<Player>();
        _showingDuration = 3;
        ShowGetMoneyButton(_showingDuration);
        Invoke(nameof(AddListenerGetMoneyButton), _showingDuration - _showingDuration / 4);
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

            _getMoneyText.text = Mathf.Round(_rewardValue).ToString();
        }
    }

    private void AddListenerGetMoneyButton()
    {
        _getMoneyButton.onClick.AddListener(OnGetMoneyButton);
    }

    private void OnGetMoneyButton()
    {
        _getMoneyButton.onClick.RemoveListener(OnGetMoneyButton);
        string rewardMoneyText = _getMoneyButton.gameObject.GetComponentInChildren<TMP_Text>().text;
        int rewardMoney;
        if (int.TryParse(rewardMoneyText, out rewardMoney) == false)
            rewardMoney = 0;

        _moneyAnimator.CreateAndAnimateMoney();
        _isResetMoney = true;
        _player.AddMoney(rewardMoney);
        
        GetMoneyButtonPressed?.Invoke();
    }

    private void ShowGetMoneyButton(float duration)
    {
        _getMoneyButtonGroup.DOFade(0, 0);
        _getMoneyButtonGroup.DOFade(1, duration);
    }
}