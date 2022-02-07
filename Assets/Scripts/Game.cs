using Cinemachine;
using UnityEngine;
using UnityEngine.Serialization;

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
        _flyingCamera.gameObject.SetActive(false);        
        _zoomCamera.gameObject.SetActive(false); 
        _battleCamera.gameObject.SetActive(true);
    }
    
    private void OnCameraFlyStarted(Transform follow)
    {
        _flyingCamera.Follow = follow;
        _flyingCamera.gameObject.transform.position = follow.position;
        _battleCamera.gameObject.SetActive(false);
        _zoomCamera.gameObject.SetActive(false);
        _flyingCamera.gameObject.SetActive(true);          
    }

    private void DoZoomOut()
    {
        _zoomCamera.gameObject.transform.position = _player.gameObject.transform.position;
        _flyingCamera.gameObject.SetActive(false);        
        _battleCamera.gameObject.SetActive(false); 
        _zoomCamera.gameObject.SetActive(true);
        Invoke(nameof(DoBattleCamera), _delayBeforeRemoteCamera);
    }

    private void DoBattleCamera()
    {
        _zoomCamera.gameObject.SetActive(false); 
        _battleCamera.gameObject.SetActive(true);          
    }
}
