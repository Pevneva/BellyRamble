using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BattleController : MonoBehaviour
{
    [SerializeField] private List<Participant> _participants;
    [SerializeField] private GameObject _winPanel;

    private Participant _currentWinner;
    private float _counter;
    private Camera _mainCamera;

    private void Start()
    {
        // _winPanel.SetActive(false); //todo uncomment
        _mainCamera = Camera.main;
        InitParticipants();
        InvokeRepeating(nameof(SetCrownToWinner), 0, 0.5f);
        _counter = 0f;
    }

    private void Update()
    {
        // Debug.Log("QAA _currentWinner : " + _currentWinner);
        // if (_counter < 0.45f)
        // {
        //     Debug.Log("QA _currentWinner : " + _currentWinner);
        //     _counter += Time.deltaTime;
        // }
        // else
        // {
        //     _counter = 0;
        // }
    }

    private void SetCrownToWinner()
    {
        foreach (var participant in _participants)
        {
            participant.TurnOffCrown();
        }

        GetWinner().TurnOnCrown();
    }

    private void InitParticipants()
    {
        _participants = FindObjectsOfType<Participant>().ToList();
    }

    private Participant GetWinner()
    {
        var currentWinner = _participants[0];
        foreach (var participant in _participants)
        {
            if (participant.Score > currentWinner.Score)
                currentWinner = participant;
        }

        return currentWinner;
    }

    public void DoImpact(Participant participant1, Participant participant2)
    {
        Debug.Log("QAZ _participants.Count : " + _participants.Count);
        bool isCameraMoving = _participants.Count > 2 ? false : true;
        // if (isCameraMoving)
        //     _mainCamera.gameObject.GetComponent<CameraMover>().SetTarget(false);
        Debug.Log("QAZ isCameraMoving : " + isCameraMoving);
        
        Participant winner = participant1.Score > participant2.Score ? participant1 : participant2;
        
        
        if (participant1 == winner)
        {
            Vector3 movingDirection =
                participant2.gameObject.transform.position - participant1.gameObject.transform.position;
            // Vector3 movingDirection = participant1 is Bot
            //     ? participant1.GetComponent<BotRuler>().MovingDirection
            //     : participant1.GetComponent<ParticipantMover>().MovingDirection;
            participant2.GetComponent<ParticipantMover>().Fly(movingDirection, isCameraMoving);
            Debug.Log("QAZ winner 1 : " + participant1);
            Debug.Log("QAZ MovingDirection 1 : " + movingDirection);
            RemoveParticipant(participant2);
            return;
        }

        if (participant2 == winner)
        {
            Vector3 movingDirection =
                participant1.gameObject.transform.position - participant2.gameObject.transform.position;
            // Vector3 movingDirection = participant2 is Bot
            //     ? participant1.GetComponent<BotRuler>().MovingDirection
            //     : participant1.GetComponent<ParticipantMover>().MovingDirection;
            
            participant1.GetComponent<ParticipantMover>().Fly(movingDirection, isCameraMoving);
            Debug.Log("QAZ winner 2 : " + participant2);
            Debug.Log("QAZ MovingDirection 2 : " + movingDirection);
            RemoveParticipant(participant1);
            return;
        }

        Debug.Log("QAZ winner 3 : " + winner);
    }

    public void RemoveParticipant(Participant participant)
    {
        _participants.Remove(participant);
    }

    public bool IsBottleEnded()
    {
        Debug.Log("QA2 participants.Count : " + _participants.Count);
        return _participants.Count <= 1;
    }

    public void ShowWinPanel()
    {
        _winPanel.SetActive(true);
        // Invoke(nameof(StopTime),2f);
    }

    private void StopTime()
    {
        Time.timeScale = 0;
    }
}