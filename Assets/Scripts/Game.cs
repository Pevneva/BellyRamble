using UnityEngine;

public class Game : MonoBehaviour
{
    [SerializeField] private Player _player;
    [SerializeField] private BattleController _battleController;
    [SerializeField] private WinPanel _winPanel;
    [SerializeField] private FXContainer _fxContainer;

    private CameraMover _cameraMover;
    
    private void Start()
    {
        HideWinPanel();
        _cameraMover = Camera.main.GetComponent<CameraMover>();
        _battleController.PlayerWon += OnPlayerWon;
        _battleController.PlayerLoosed += OnPlaeyrLoosed;
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
        Invoke(nameof(SetNewLevel), 3.5f);
    }

    public void HideWinPanel()
    {
        _winPanel.gameObject.SetActive(false);
    }

    private void OnPlaeyrLoosed()
    {
        //to do Loose Panel
        Debug.Log("Game Over");
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
}
