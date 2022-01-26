using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{
    [SerializeField] private Player _player;
    [SerializeField] private BattleController _battleController;
    [SerializeField] private WinPanel _winPanel;
    [SerializeField] private FXContainer _fxContainer;

    private Camera _camera;
    private CameraMover _cameraMover;
    
    private void Start()
    {
        HideWinPanel();
        _camera = Camera.main;
        _cameraMover = _camera.GetComponent<CameraMover>();
        _battleController.PlayerWon += OnWinPanelShown;
    }

    public void ShowWinPanel()
    {
        _winPanel.gameObject.SetActive(true);
        _fxContainer.ShowWinPanelFx();
    }

    public void HideWinPanel()
    {
        _winPanel.gameObject.SetActive(false);
    }

    private void OnWinPanelShown()
    {
        _winPanel.GetMoneyButtonPressed += OnGetMoneyButtonPressed;
        _battleController.PlayerWon -= OnWinPanelShown;
    }

    private void OnGetMoneyButtonPressed()
    {
        Invoke(nameof(SetNewLevel), 3.5f);
    }
    
    private void SetNewLevel()
    {
        _player.ResetPlayer();
        _cameraMover.SetPosition(_player.transform.position);
        _cameraMover.SetTarget(_player.transform);
        _cameraMover.SetKindMoving(true);
        Invoke(nameof(HideWinPanel), 0);
        _cameraMover.ZoomIn();
        _cameraMover.ZoomOut();
    }

    public void EndBottle()
    {
        Invoke(nameof(ShowWinPanel), _battleController.ParticipantFlyingTime);
    }
}
