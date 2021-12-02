using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerDataView : MonoBehaviour
{
    [SerializeField] private TMP_Text _score;
    [SerializeField] private float _offsetY;

    public void Render(Participant participant)
    {
        _score.text = participant.Score.ToString();
        SetPosition(participant, _offsetY);
    }

    private void SetPosition(Participant participant, float offsetY)
    {
        Vector3 position = participant.transform.position;
        Vector3 positionOnScreen = Camera.main.WorldToScreenPoint(position);
        
        positionOnScreen.y += offsetY;
        transform.position = positionOnScreen;
    }
}
