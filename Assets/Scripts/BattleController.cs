using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class BattleController : MonoBehaviour
{
    [SerializeField] private List<Participant> _participants;
    [SerializeField] private Game _game;
    // [SerializeField] private float _participantFlyingTime;

    // public float ParticipantFlyingTime => _participantFlyingTime;
    public event UnityAction PlayerWon; 
    public event UnityAction PlayerLoosed; 
    
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
            participant2.GetComponent<ParticipantFlyer>().Fly(movingDirection, isBottleWillBeEnded);
            RemoveParticipant(participant2);
        } else if (participant2 == winner)
        {
            Vector3 movingDirection = participant1.gameObject.transform.position - participant2.gameObject.transform.position;
            participant1.GetComponent<ParticipantFlyer>().Fly(movingDirection, isBottleWillBeEnded);
            RemoveParticipant(participant1);
        }

        var isPlayerLoosed = winner is Player == false;

        if (isPlayerLoosed)
        {
            PlayerLoosed?.Invoke();
        }
        else if (isBottleWillBeEnded)
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

    public Participant GetNearestParticipant(Participant target)
    {
        var listTargetParticipants = _participants;
        var exceptedParticipant = listTargetParticipants
            .Where(p => p == gameObject.GetComponent<Bot>() || p == gameObject.GetComponent<Player>());

        listTargetParticipants = listTargetParticipants.Except(exceptedParticipant).ToList();
        
        Participant targetParticipant = listTargetParticipants[0];
        var distanceTarget = Vector3.Distance(transform.position, targetParticipant.gameObject.transform.position);
        foreach (var participant in listTargetParticipants)
        {
            var distance = Vector3.Distance(transform.position, participant.gameObject.transform.position);
            if (distance < distanceTarget)
            {
                targetParticipant = participant;
                distanceTarget = distance;
            }
        }
        return targetParticipant;
    }
}