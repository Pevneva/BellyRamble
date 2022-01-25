using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FXContainer : MonoBehaviour
{
    [SerializeField] private GameObject _winPanelFx;

    public void ShowWinPanelFx()
    {
        _winPanelFx.SetActive(true);
    }
    
    public void HideWinPanelFx()
    {
        _winPanelFx.SetActive(false);
    }
}
