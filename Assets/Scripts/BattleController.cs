using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class BattleController : MonoBehaviour
{
    [SerializeField] private List<Participant> _participants;
    [SerializeField] private Game _game;

    [SerializeField] private float _participantFlyingTime;
    // [SerializeField] private GameObject _winPanel;

    public float ParticipantFlyingTime => _participantFlyingTime;

    private Participant _currentWinner;
    private float _counter;
    // private Camera _mainCamera;
    
    public event UnityAction PlayerWon; 

    private void Start()
    {
        // _winPanel.SetActive(false); //todo uncomment
        // _mainCamera = Camera.main;
        InitParticipants();
        InvokeRepeating(nameof(SetCrownToWinner), 0, 0.5f);
        _counter = 0f;
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
        bool isBottleWillBeEnded = _participants.Count > 2 ? false : true;
        
        Participant winner = participant1.Score > participant2.Score ? participant1 : participant2;
        
        if (participant1 == winner)
        {
            Vector3 movingDirection =
                participant2.gameObject.transform.position - participant1.gameObject.transform.position;
            participant2.GetComponent<ParticipantMover>().Fly(movingDirection, isBottleWillBeEnded);
            
            RemoveParticipant(participant2);
        } else if (participant2 == winner)
        {
            Vector3 movingDirection =
                participant1.gameObject.transform.position - participant2.gameObject.transform.position;
            // Vector3 movingDirection = participant2 is Bot
            //     ? participant1.GetComponent<BotRuler>().MovingDirection
            //     : participant1.GetComponent<ParticipantMover>().MovingDirection;
            
            participant1.GetComponent<ParticipantMover>().Fly(movingDirection, isBottleWillBeEnded);
            RemoveParticipant(participant1);
        }

        if (isBottleWillBeEnded) //&& winner is Player)
        {
            _game.EndBottle();
            PlayerWon?.Invoke();
        } 
    }

    public void RemoveParticipant(Participant participant)
    {
        _participants.Remove(participant);
    }

    public bool IsBottleEnded()
    {
        return _participants.Count <= 1;
    }

    public void ShowWinPanel()
    {
        // _winPanel.SetActive(true);
        // Invoke(nameof(StopTime),2f);
    }

    private void StopTime()
    {
        Time.timeScale = 0;
    }
}