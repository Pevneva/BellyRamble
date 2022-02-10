using System;
using Cinemachine;
using UnityEngine;

public class Game : MonoBehaviour
{
    [SerializeField] private Player _player;
    [SerializeField] private BattleController _battleController;
    [SerializeField] private WinPanel _winPanel;
    [SerializeField] private FXContainer _fxContainer;
    [SerializeField] private CinemachineVirtualCamera _battleCamera;
    [SerializeField] private CinemachineVirtualCamera _flyingCamera;
    [SerializeField] private CinemachineVirtualCamera _zoomCamera;

    private readonly float _delayAfterGetMoneyPressed = 3.5f;
    private readonly float _delayBeforeRemoteCamera = 0.5f;
    
    private void Start()
    {
        HideWinPanel();
        SetStartCamera();
        _battleController.PlayerWon += OnPlayerWon;
        _battleController.PlayerLoosed += OnPlayerLoosed;
        _battleController.CameraFlyStarted += OnCameraFlyStarted;
    }

    private void OnPlayerWon()
    {
        Invoke(nameof(ShowWinPanel), MovingParamsController.FlyingTime);
        _battleController.PlayerWon -= OnPlayerWon;
    }

    public void ShowWinPanel()
    {
        _winPanel.GetMoneyButtonPressed += OnGetMoneyButtonPressed;
        _winPanel.gameObject.SetActive(true);
        _fxContainer.ShowWinPanelFx();
    }

    private void OnGetMoneyButtonPressed()
    {
        Invoke(nameof(SetNewLevel), _delayAfterGetMoneyPressed);
        _winPanel.GetMoneyButtonPressed -= OnGetMoneyButtonPressed;
    }

    public void HideWinPanel()
    {
        _winPanel.gameObject.SetActive(false);
    }

    private void OnPlayerLoosed()
    {
        //to do Loose Panel
        _battleController.PlayerLoosed -= OnPlayerLoosed;
    }

    private void SetNewLevel()
    {
        _player.ResetPlayer();
        DoZoomOut();
        HideWinPanel();
    }

    private void SetStartCamera()
    {
        SetActiveCamera(_battleCamera,_flyingCamera, _zoomCamera);
    }
    
    private void OnCameraFlyStarted(Transform follow)
    {
        _flyingCamera.Follow = follow;
        _flyingCamera.gameObject.transform.position = follow.position;
        SetActiveCamera(_flyingCamera, _battleCamera, _zoomCamera);
    }

    private void DoZoomOut()
    {
        _zoomCamera.gameObject.transform.position = _player.gameObject.transform.position;
        SetActiveCamera(_zoomCamera, _flyingCamera, _battleCamera);
        Invoke(nameof(DoBattleCamera), _delayBeforeRemoteCamera);
    }

    private void DoBattleCamera()
    {
        SetActiveCamera(_battleCamera, _zoomCamera);
    }

    private void SetActiveCamera(CinemachineVirtualCamera activeCamera, params CinemachineVirtualCamera[] inactiveCameras)
    {
        activeCamera.gameObject.SetActive(true);
        foreach (var inactiveCamera in inactiveCameras)
            inactiveCamera.gameObject.SetActive(false);
    }
}
