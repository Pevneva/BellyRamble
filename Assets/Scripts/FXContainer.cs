using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FXContainer : MonoBehaviour
{
    [SerializeField] private GameObject _lefLegParticipant;
    [SerializeField] private GameObject _rightLegParticipant;
    [SerializeField] private GameObject _backParticipant;

    public void PlayParticipantEffects()
    {
        _lefLegParticipant.SetActive(true);
        _rightLegParticipant.SetActive(true);
        _backParticipant.SetActive(true);
    }

    public void StopParticipantEffects()
    {
        _lefLegParticipant.SetActive(false);
        _rightLegParticipant.SetActive(false);
        _backParticipant.SetActive(false);        
    }
}
