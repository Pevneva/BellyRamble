using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class BattleController : MonoBehaviour
{
    [SerializeField] private List<Participant> _participants;
    [SerializeField] private Game _game;
    [SerializeField] private float _participantFlyingTime;

    public float ParticipantFlyingTime => _participantFlyingTime;
    public event UnityAction PlayerWon; 
    
    private void Start()
    {
        InitParticipants();
        InvokeRepeating(nameof(SetCrownToWinner), 0, 0.5f);
    }

    private void SetCrownToWinner()
    {
        foreach (var participant in _participants)
        {
            participant.SetCrownVisibility(false);
        }

        GetWinner().SetCrownVisibility(true);
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
            Vector3 movingDirection = participant2.gameObject.transform.position - participant1.gameObject.transform.position;
            participant2.GetComponent<ParticipantMover>().Fly(movingDirection, isBottleWillBeEnded);
            RemoveParticipant(participant2);
        } else if (participant2 == winner)
        {
            Vector3 movingDirection = participant1.gameObject.transform.position - participant2.gameObject.transform.position;
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
}