using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class BattleController : MonoBehaviour
{
    [SerializeField] private List<Participant> _participants;
    [SerializeField] private BorderChecker _borderChecker;
    [SerializeField] private FoodUtils _foodUtils;

    public event UnityAction PlayerWon; 
    public event UnityAction PlayerLoosed;
    public event UnityAction<Transform> CameraFlyStarted;

    private void Awake()
    {
        InvokeRepeating(nameof(SetCrownToWinner), 0, 0.5f);

        foreach (var participant in _participants)
        {
            participant.Init(this);
            participant.GetComponent<ParticipantMover>().Init(_borderChecker);
            if (participant.gameObject.TryGetComponent<BotRuler>(out BotRuler botRuler))
                botRuler.Init(this, _borderChecker, _foodUtils);
        }
    }

    private void SetCrownToWinner()
    {
        foreach (var participant in _participants)
            participant.SetCrownVisibility(false);

        GetWinner().SetCrownVisibility(true);
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
        Participant looser = participant1.Score <= participant2.Score ? participant1 : participant2;
        
        Vector3 flyingDirection = looser.gameObject.transform.position - winner.gameObject.transform.position;
        looser.GetComponent<ParticipantFlyer>().Fly(flyingDirection);
        if (isBottleWillBeEnded)
            FlyingCameraStart(looser.gameObject.transform);
        RemoveParticipant(looser);

        var isPlayerLoosed = winner is Player == false;

        if (isPlayerLoosed)
            PlayerLoosed?.Invoke();
        else if (isBottleWillBeEnded)
            PlayerWon?.Invoke();
    }
    
    private void RemoveParticipant(Participant participant)
    {
        _participants.Remove(participant);
    }

    public Participant GetNearestParticipant(Participant baseParticipant)
    {
        var listTargetParticipants = _participants.Except(_participants
            .Where(p => p == baseParticipant)).ToList();
        
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

    public void FlyingCameraStart(Transform transform)
    {
        CameraFlyStarted?.Invoke(transform);
    }
}