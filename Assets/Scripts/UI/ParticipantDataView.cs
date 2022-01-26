using System;
using TMPro;
using UnityEngine;

public class ParticipantDataView : MonoBehaviour
{
    [SerializeField] private TMP_Text _score;
    [SerializeField] private float _offsetY;
    [SerializeField] private RectTransform _area;

    public void Render(Participant participant)
    {
        _score.text = participant.Score.ToString();
        SetPosition(participant, _offsetY);
    }

    private void SetPosition(Participant participant, float offsetY)
    {
        Camera camera = Camera.main;
        Vector3 worldPosition = participant.transform.position;

        Vector3 screenPoint = camera.WorldToScreenPoint(worldPosition);
        screenPoint.z = 0;
        Vector2 screenPos;
        
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(_area, screenPoint, camera, out screenPos))
        {
            transform.localPosition = screenPos;
            return;
        }
        else
        {
            throw new Exception();
        }
    }
}
